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
        Rigidbody2D player = collision.gameObject.GetComponent<Rigidbody2D>();
        if (player != null)
        {
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            float randomForce = 10f;
            player.AddForce(randomDirection * randomForce, ForceMode2D.Impulse);
        }
        
    }
}