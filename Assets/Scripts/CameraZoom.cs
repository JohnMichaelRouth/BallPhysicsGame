using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Rigidbody2D player;
    public float terminalVelocity = 20f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSmoothTime = 0.2f; // Time taken to reach the desired zoom level
    public float heightFactor = 0.5f; // How much the height influences zooming
    public float maxHeight = 150f;
    public float positionSmoothTime = 0.2f; // Time taken for camera to follow the player's position

    private Camera cam;
    private float zoomVelocity = 0f;  // Used internally by SmoothDamp for zoom
    private Vector2 cameraVelocity = Vector2.zero; // Used internally by SmoothDamp for position

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleZoom();
        HandlePosition();
    }

    void HandleZoom()
    {
        float currentSpeed = player.velocity.magnitude;
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / terminalVelocity);
        float normalizedHeight = Mathf.Clamp01(player.transform.position.y / maxHeight);
        // Get a zoom based on both the player speed and player height
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, normalizedSpeed + normalizedHeight * heightFactor);
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        // Adjust the zoom with SmoothDamp
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime);
    }

    void HandlePosition()
    {
        // The target position is the player's position but with the camera's current z-coordinate
        Vector2 targetPosition = player.position;
        Vector2 smoothedPosition = Vector2.SmoothDamp(transform.position, targetPosition, ref cameraVelocity, positionSmoothTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
