using UnityEngine;

public class RandomDirectionEnemy : Enemy
{
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

        //Bounce the Player in a random direction
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb != null && playerRb.bodyType == RigidbodyType2D.Dynamic)
        {
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            float randomForce = 10f;
            playerRb.AddForce(randomDirection * randomForce, ForceMode2D.Impulse);
        }
        
    }
}