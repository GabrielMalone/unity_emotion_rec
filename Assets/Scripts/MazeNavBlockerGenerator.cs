using UnityEngine;
using Unity.AI.Navigation;

public class MazeNavBlockerGenerator : MonoBehaviour
{
    public PolygonCollider2D mazeCollider;

    [Header("Blocker Settings")]
    public float wallThickness = 0.2f;
    public float depth = 1f;

    [ContextMenu("Generate NavMesh Blockers")]
    public void GenerateBlockers()
    {
        if (mazeCollider == null)
            mazeCollider = GetComponent<PolygonCollider2D>();

        if (mazeCollider == null)
        {
            Debug.LogError("No PolygonCollider2D found.");
            return;
        }

        Transform oldParent = transform.Find("GeneratedNavBlockers");

        if (oldParent != null)
            DestroyImmediate(oldParent.gameObject);

        GameObject blockerParent =
            new GameObject("GeneratedNavBlockers");

        blockerParent.transform.SetParent(transform);
        blockerParent.transform.localPosition = Vector3.zero;
        blockerParent.transform.localRotation = Quaternion.identity;
        blockerParent.transform.localScale = Vector3.one;

        for (int pathIndex = 0;
             pathIndex < mazeCollider.pathCount;
             pathIndex++)
        {
            Vector2[] path = mazeCollider.GetPath(pathIndex);

            for (int i = 0; i < path.Length; i++)
            {
                Vector2 start = path[i];
                Vector2 end = path[(i + 1) % path.Length];

                CreateBlocker(
                    blockerParent.transform,
                    start,
                    end,
                    pathIndex,
                    i
                );
            }
        }
    }

    void CreateBlocker(
        Transform parent,
        Vector2 start,
        Vector2 end,
        int pathIndex,
        int segmentIndex)
    {
        Vector2 center = (start + end) * 0.5f;

        Vector2 direction = end - start;

        float length = direction.magnitude;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        GameObject blocker = new GameObject(
            $"Blocker_{pathIndex}_{segmentIndex}"
        );

        blocker.transform.SetParent(parent);

        blocker.transform.localPosition = center;

        blocker.transform.localRotation =
            Quaternion.Euler(0, 0, angle);

        NavMeshModifierVolume volume =
            blocker.AddComponent<NavMeshModifierVolume>();

        volume.area = 1; // Not Walkable

        volume.size = new Vector3(
            length,
            wallThickness,
            depth
        );
    }
}