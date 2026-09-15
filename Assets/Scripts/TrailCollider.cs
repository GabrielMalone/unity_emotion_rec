using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : MonoBehaviour
{
    public Transform player;
    public float pointSpacing = 0.25f;
    public PhysicsMaterial2D trailPhysicsMaterial;

    

    private EdgeCollider2D edgeCollider;
    private List<Vector2> points = new List<Vector2>();

    [Header("Slug Trail Length")]
    public int maxPoints = 5000;

    void Start()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.sharedMaterial = trailPhysicsMaterial;
        AddPoint();
    }

    void Update()
    {
        if (Vector2.Distance(points[points.Count - 1], player.position) >= pointSpacing)
        {
            AddPoint();
        }
    }

    void AddPoint()
    {
        Vector2 localPoint = transform.InverseTransformPoint(player.position);

        points.Add(localPoint);

        if (points.Count > maxPoints)
        {
            points.RemoveAt(0);
        }

        edgeCollider.points = points.ToArray();
    }
}