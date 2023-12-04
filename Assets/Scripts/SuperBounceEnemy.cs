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
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb != null && playerRb.bodyType == RigidbodyType2D.Dynamic)
        {
            float upwardForce = 10f;
            playerRb.velocity = new Vector3(playerRb.velocity.x * 0.5f, 0, 0); // set y velocity to 0 and halve x velocity
            playerRb.AddForce(Vector2.up * upwardForce * superBounceStrength, ForceMode2D.Impulse);
        }

    }
}