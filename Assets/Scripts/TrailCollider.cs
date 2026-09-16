using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : MonoBehaviour
{
    public Transform player;
    public float pointSpacing = 0.25f;

    public static bool slimeTrailEnabled = false;

    [Header("Physics")]
    public PhysicsMaterial2D trailPhysicsMaterial;

    [Header("Slime Visuals")]
    public Material slimeMaterial;
    public float slimeWidth = 0.5f;

    [Header("Slug Trail Length")]
    public int maxPoints = 5000;

    private EdgeCollider2D edgeCollider;
    public LineRenderer lineRenderer;
    private List<Vector2> points = new List<Vector2>();

    void Start()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
   
        edgeCollider.sharedMaterial = trailPhysicsMaterial;

        lineRenderer.material = slimeMaterial;
        lineRenderer.startWidth = slimeWidth;
        lineRenderer.endWidth = slimeWidth;
        lineRenderer.useWorldSpace = false;
        lineRenderer.sortingOrder = -1;


        AddPoint();
    }

    void Update()
    {
        Vector2 playerLocalPosition = transform.InverseTransformPoint(player.position);

        if (points.Count == 0 ||
            Vector2.Distance(points[points.Count - 1], playerLocalPosition) >= pointSpacing)
        {
            AddPoint();
        }
    }

    void AddPoint()
    {
        
        if (!slimeTrailEnabled) return;

        Vector2 localPoint = transform.InverseTransformPoint(player.position);

        points.Add(localPoint);

        if (points.Count > maxPoints)
        {
            points.RemoveAt(0);
        }

        // Collision trail
        edgeCollider.points = points.ToArray();

        // Visible slime trail
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(
                i,
                new Vector3(points[i].x, points[i].y, 0)
            );
        }
    }
}