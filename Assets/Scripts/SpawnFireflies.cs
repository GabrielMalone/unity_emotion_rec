using UnityEngine;
using GSM = GameStateManagerScript;
public class SpawnFireflies : MonoBehaviour
{
    [Header("Firefly Spawn Settings")]
    public CircleCollider2D spawnArea;
    public CircleCollider2D lvl2SpawnArea;
    public GameObject Firefly;
    public int maxNumFireflies = 100;
    public GameObject [] fireflies;
   
    // --------------------------------------------------------------------------------------------
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }
    // --------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

    }
    // --------------------------------------------------------------------------------------------
    Vector3 GetRandomSpawnPoint()
    {
        if (GSM.Instance.lvl2) spawnArea = lvl2SpawnArea;
        Vector2 randomPoint = Random.insideUnitCircle * spawnArea.radius;
        return spawnArea.transform.TransformPoint(randomPoint);
    }
    // --------------------------------------------------------------------------------------------
    public void Spawn()
    {
        for (int i = 0 ; i < fireflies.Length ; i ++)
        {
            fireflies[i] = Instantiate(Firefly, GetRandomSpawnPoint(), Quaternion.identity);
        }
    }
    // --------------------------------------------------------------------------------------------
}
