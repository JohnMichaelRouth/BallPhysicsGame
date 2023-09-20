using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTrail : MonoBehaviour
{
    private Rigidbody2D player;
    private TrailRenderer trail;
    private CircleCollider2D circleCollider;

    public float terminalVelocity = 20f; 

    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        float currentSpeed = player.velocity.magnitude;

        // Modify the trail's width based on the players speed
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / terminalVelocity);
        trail.startWidth = Mathf.Lerp(0, circleCollider.radius, normalizedSpeed);

        // Trail gets a bit longer whenever going fast
        trail.time = Mathf.Lerp(1f, 1.5f, normalizedSpeed);
    }
}
