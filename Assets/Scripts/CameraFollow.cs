using UnityEngine;

public class CameraFollow : MonoBehaviour
{

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
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
