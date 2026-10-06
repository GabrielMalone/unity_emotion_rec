using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera cam;
    public Rigidbody2D playerRb;

    public float minZoom = 5f;       // Zoom when moving slowly
    public float maxZoom = 12f;      // Zoom when moving fast
    public float speedForMaxZoom = 12f;
    public float zoomSmoothSpeed = 2f;

    public Transform target;     // Drag your player here
    public Vector3 offset;       // Distance away from the player (e.g., X:0, Y:2, Z:-10)
    public float smoothSpeed = 0.125f; // Higher values mean faster catch-up

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Follow player
            Vector3 desiredPosition = target.position + offset;

            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed
            );

            transform.position = smoothedPosition;


            // // Camera zoom based on player speed
            // float speed = playerRb.linearVelocity.magnitude;

            // float speedAmount = Mathf.InverseLerp(
            //     0f,
            //     speedForMaxZoom,
            //     speed
            // );

            // float targetZoom = Mathf.Lerp(
            //     minZoom,
            //     maxZoom,
            //     speedAmount
            // );

            // cam.orthographicSize = Mathf.Lerp(
            //     cam.orthographicSize,
            //     targetZoom,
            //     Time.deltaTime * zoomSmoothSpeed
            // );
        }
    }
}
