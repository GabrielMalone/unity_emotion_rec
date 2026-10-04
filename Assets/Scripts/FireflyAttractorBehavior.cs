using UnityEngine;
using UnityEngine.AI;

public class FireflyAttractorBehavior : MonoBehaviour
{
    [Header("Firefly Attractor Settings")]
    public CircleCollider2D environmentArea;
    public Transform firstStatue;
    public NavMeshAgent agent;

    private Vector3 destination;

    private float timeSinceMove;
    private float maxTravelTime = 5f;

    void Start()
    {

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        GoToStatue();
        //MoveToRandomLocation();
    }

    // void Update()
    // {
    //     // Reached destination
    //     if (!agent.pathPending &&
    //         agent.remainingDistance <= agent.stoppingDistance)
    //     {
    //         MoveToRandomLocation();
    //     }

    //     // Something prevented us from reaching it — abandon it
    //     else if (Time.time - timeSinceMove > maxTravelTime)
    //     {
    //         MoveToRandomLocation();
    //     }
    // }

    void GoToStatue()
    {
        agent.SetDestination(firstStatue.position);
    }

    void MoveToRandomLocation()
    {
        Vector2 rndPosition =
            (Vector2)environmentArea.transform.position +
            Random.insideUnitCircle * environmentArea.radius;

        destination = rndPosition;
        agent.SetDestination(destination);

        timeSinceMove = Time.time;
    }
}