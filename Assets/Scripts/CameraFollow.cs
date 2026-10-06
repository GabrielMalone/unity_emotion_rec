using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    [Header("Zoom")]
    public float normalZoom = 0.5f;
    public float zoomedIn = 5f;
    public float zoomSpeed = 3f;

    private float targetZoom;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = normalZoom;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed
            );

            transform.position = smoothedPosition;
        }

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );
    }

    public void ZoomIn()
    {
        targetZoom = zoomedIn;
    }

    public void ZoomOut()
    {
        targetZoom = normalZoom;
    }
}