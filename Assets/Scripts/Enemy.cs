using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private ParticleSystem particle;
    private SpriteRenderer sr;
    public CircleCollider2D enemyCollider;
    public float despawnRadius = 350.0f;
    private Transform playerTransform;
    public int scoreValue = 100;

    public AudioSource enemyAudioSource;
    public AudioClip deathSound;

    public GameObject scorePopupPrefab;
    public Sprite[] scoreSprites; // assigned in editor

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        particle = GetComponentInChildren<ParticleSystem>();
        enemyCollider = GetComponent<CircleCollider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Check distance from player and despawn if too far
        if (Mathf.Abs(playerTransform.position.x - transform.position.x) > despawnRadius)
        {
            Destroy(this.gameObject);
            GameManager.instance.EnemyDespawned();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
        }
    }

    protected virtual void HandlePlayerCollision(Collision2D collision)
    {
        // Immediately deactivate the collider so it won't be hit again
        enemyCollider.enabled = false;

        // Play death sound
        enemyAudioSource.PlayOneShot(deathSound);

        // Kill this enemy
        StartCoroutine(EnemyDie());

        // Update the GameManager
        GameManager.instance.EnemyKilled(scoreValue);

        // Bounce the player up
        Rigidbody2D player = collision.gameObject.GetComponent<Rigidbody2D>();
        if (player != null)
        {
            float upwardForce = 10f;
            player.velocity = new Vector3(player.velocity.x * 0.5f, 0, 0); // set y velocity to 0 and halve x velocity
            player.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
        }
    }

    protected IEnumerator EnemyDie()
    {
        ShowScorePopup(scoreValue);

        //display particles
        particle.Play();

        //disable sprite
        sr.enabled = false;

        //wait for particles
        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);

        // Kill this enemy
        Destroy(this.gameObject);
    }

    void ShowScorePopup(int score)
    {
        GameObject scorePopup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);
        ScorePopup popupScript = scorePopup.GetComponent<ScorePopup>();

        // Assuming score values are in hundreds and sprites are in the order of 100, 200, ..., 9000
        int spriteIndex = Mathf.Max(0, ComboManager.instance.GetComboCount() - 1);
        popupScript.SetScoreSprite(scoreSprites[spriteIndex]);
    }
}

