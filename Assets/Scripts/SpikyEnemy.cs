using UnityEngine;

public class SpikyEnemy : Enemy
{
    protected override void HandlePlayerCollision(Collision2D collision)
    {
        // Player dies when they hit spiky enemy
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.KillPlayer();
        }
    }
}