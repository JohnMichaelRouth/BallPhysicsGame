using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBounceEnemy : Enemy
{
    public float superBounceStrength = 2f;

    protected override void HandlePlayerCollision(Collision2D collision)
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
            player.AddForce(Vector2.up * upwardForce * superBounceStrength, ForceMode2D.Impulse);
        }

    }
}