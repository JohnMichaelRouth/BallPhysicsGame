using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*
 * I basically tried to recreate the game that made me want to get into game dev back in 2019
 * There was one video I watched where someone was making a game in 12 hours and it was really interesting
 * https://youtu.be/mFNrOGPVls0?si=LI1yeNfz74MTtT5E
 * This is the video of the game^
 * The creator did not release source code so this was mostly me trying to discover how things
 * in this game worked and it was a fun project
 * I hope that recreating a game this closely is okay as it was useful to have a goal/reference!!
 */
public class PlayerController : MonoBehaviour
{
    private ParticleSystem particle;
    private SpriteRenderer sr;

    public float launchPower = 2.0f;
    public float slowMotionFactor = 0.1f; // 1/10 Speed
    public int maxLaunches = 1;
    public int killHeight = -2;
    private int currentLaunches = 0;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Rigidbody2D body;
    private bool isDragging = false;
    private LineRenderer lineRenderer;
    private TrailRenderer trailRenderer;
    private Vector3 startPosition;

    [Header("Sprite Deformation")]
    public Transform spriteTransform; // Seperated the sprite just like in class
    public float deformationFactor = 0.5f;  // Maximum stretch/squish
    public float minVelocityForDeformation = 1f; 
    public float maxDragForDeformation = 10f; // Drag distance at which maximum deformation is achieved

    [Header("Chromatic Aberration")]
    public float chromAbIntWhileDrag = 0.3f;
    public float defaultChromAb = 0.05f;
    private ChromaticAberration chromaticAberration;

    [Header("Health")]
    public float maxHealth = 10;
    private float health;

    [Header("AudioFiltering")]
    public AudioSource backgroundMusic;
    public AudioLowPassFilter lowPassFilter;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        particle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        startPosition = transform.position;

        Volume volume = FindObjectOfType<Volume>();
        if (volume.profile.TryGet(out chromaticAberration))
        {
            chromaticAberration.intensity.Override(defaultChromAb);
        }

        health = maxHealth;
    }

    private void Update()
    {
        if (!GameManager.instance.GetOnMenu())
        {
            health -= Time.deltaTime;
            if(health <= 0f)
            {
                health = 0f;
                KillPlayer();
            }
            GameManager.instance.UpdateHealthBar(health,maxHealth);
        }
        

        // Start the drag
        if (Input.GetMouseButtonDown(0) && currentLaunches < maxLaunches && !GameManager.instance.GetOnMenu())
        {
            isDragging = true;
            Time.timeScale = slowMotionFactor; // Slow down time when starting to drag
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            chromaticAberration.intensity.Override(chromAbIntWhileDrag);
            lowPassFilter.cutoffFrequency = 1000;
        }

        // End the drag and launch the player
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Time.timeScale = 1.0f; // Reset time back to normal when releasing the drag
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Launch();

            chromaticAberration.intensity.Override(defaultChromAb);
            lowPassFilter.cutoffFrequency = 22000;
        }

        // Logic for trajectory line
        if (isDragging)
        {
            startPoint = transform.position; // Set the start point to the player's current position
            Vector2 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DrawTrajectory(startPoint, currentPoint);
        }
        else
        {
            lineRenderer.positionCount = 0; // Clear the trajectory line when not dragging
        }

        // Ball deformation logic:
        if (isDragging)
        {
            Vector2 dragDirection = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - startPoint;
            float dragAngle = Mathf.Atan2(dragDirection.y, dragDirection.x) * Mathf.Rad2Deg;
            spriteTransform.rotation = Quaternion.Euler(0, 0, dragAngle + 90); // + 90 to invert direction

            float dragDistance = dragDirection.magnitude;
            float dragDeformation = Mathf.Clamp(dragDistance / maxDragForDeformation, 0, deformationFactor);
            float squishScale = 1 - dragDeformation;
            float stretchScale = 1 / squishScale;
            spriteTransform.localScale = new Vector3(stretchScale, squishScale, 1);
        }
        else
        {
            // If velocity is below the threshold, reset the sprite's scale and end the function
            if (body.velocity.magnitude < minVelocityForDeformation)
            {
                spriteTransform.localScale = Vector3.one;
                return;
            }

            float velocityAngle = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
            spriteTransform.rotation = Quaternion.Euler(0, 0, velocityAngle);

            float normalizedVelocity = Mathf.InverseLerp(minVelocityForDeformation, 20, body.velocity.magnitude);
            float velocityDeformation = Mathf.Lerp(0, deformationFactor, normalizedVelocity);

            float stretch = 1 + velocityDeformation;
            float squish = 1 / stretch;
            spriteTransform.localScale = new Vector3(stretch, squish, 1);
        }

        //in case the player passes through the lava, this is a backup
        if (transform.position.y < killHeight)
        {
            KillPlayer();
        }
    }

    private void Launch()
    {
        Vector2 direction = startPoint - endPoint; 
        float distance = direction.magnitude;

        // Set the velocity to zero, was getting an inaccurate trajectory without this
        body.velocity = Vector2.zero;

        // Apply force to the player's Rigidbody2D
        body.AddForce(direction.normalized * distance * launchPower, ForceMode2D.Impulse);

        currentLaunches++;
    }

    /*
     * Referenced https://schatzeder.medium.com/basic-trajectory-prediction-in-unity-8537b52e1b34
     */
    private void DrawTrajectory(Vector2 start, Vector2 end)
    {
        Vector2 direction = start - end;
        float distance = direction.magnitude;
        Vector2 force = direction.normalized * distance * launchPower;

        int resolution = 10; // Number of segments in the trajectory line
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.SetPosition(0, start);

        Vector2 currentPoint = start;
        Vector2 previousPoint = start;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = (i / (float)resolution) * 2; // Arbitrary time for simulation
            currentPoint.x = start.x + force.x / body.mass * simulationTime;
            currentPoint.y = start.y + force.y / body.mass * simulationTime - 0.5f * Mathf.Abs(Physics2D.gravity.y) * Mathf.Pow(simulationTime, 2);
            lineRenderer.SetPosition(i, currentPoint);
            previousPoint = currentPoint;
        }
    }

    public void KillPlayer()
    {
        health = 0f;

        body.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);

        //display particles
        particle.Play();

        //disable sprite
        sr.enabled = false;

        trailRenderer.enabled = false;

        body.velocity = Vector2.zero;

        body.gravityScale = 0f;

        GameManager.instance.PlayerDied();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        // Tags are really useful and I'm glad I found them this early  s
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Wall"))
        {
            currentLaunches = 0; // Get your jumps back :D
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health = maxHealth;
        }
    }

    public void RestartGame()
    {
        health = maxHealth;
        transform.position = startPosition;
        sr.enabled = true;
        trailRenderer.enabled = true;
        body.gravityScale = 1f;
    }
}
