using UnityEngine;

public class SpawnFireflies : MonoBehaviour
{
    [Header("Firefly Spawn Settings")]
    public CircleCollider2D spawnArea;
    public GameObject Firefly;
    public int maxNumFireflies = 100;
    public GameObject [] fireflies;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    Vector3 GetRandomSpawnPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnArea.radius;
        return spawnArea.transform.TransformPoint(randomPoint);
    }

    void Spawn()
    {
        for (int i = 0 ; i < fireflies.Length ; i ++)
        {
            fireflies[i] = Instantiate(Firefly, GetRandomSpawnPoint(), Quaternion.identity);
        }
    }

}
