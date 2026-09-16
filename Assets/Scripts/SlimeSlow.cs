using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
 
// Attach this to the same object as TrailCollider (the one with the EdgeCollider2D).
// Make sure enemies are tagged "Enemy" and the EdgeCollider2D is NOT set to "Is Trigger"
// if you still want it to physically block movement — this uses collision, not trigger.
public class SlimeSlow : MonoBehaviour
{
    [Header("Slow Settings")]
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.00f; 
    public string enemyTag = "Enemies";
    public float spinForce = 1f;
    public float brakingForce = 5f;

    // Track each enemy's original speed so we can restore it exactly, even with multiple enemies
    private Dictionary<NavMeshAgent, float> originalSpeeds = new Dictionary<NavMeshAgent, float>();
 
 
    // If you end up using a trigger collider instead, these will handle it too
    private void OnTriggerEnter2D(Collider2D other)
    {
        TrySlow(other.gameObject);
      
    }
 
    private void OnTriggerExit2D(Collider2D other)
    {
        TryRestore(other.gameObject);
    }
 
    private void TrySlow(GameObject obj)
    {
        if (!obj.CompareTag(enemyTag)) return;
        Debug.Log("enemy ENTERING collision");

        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        if (agent == null) return;
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Only store the original speed the first time we slow this agent
        if (!originalSpeeds.ContainsKey(agent))
        {
            originalSpeeds[agent] = agent.speed;
            agent.speed = agent.speed * slowMultiplier;
            // Vector2 oppositeDirection = -rb.linearVelocity.normalized;
            // rb.AddForce(oppositeDirection * brakingForce, ForceMode2D.Impulse);

        }
        // also let's spint out the enemy

        rb.AddTorque(spinForce, ForceMode2D.Impulse);
    }
 
    private void TryRestore(GameObject obj)
    {
        if (!obj.CompareTag(enemyTag)) return;
 
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        Debug.Log("enemy EXITING collision");
        if (originalSpeeds.TryGetValue(agent, out float originalSpeed))
        {
            agent.speed = originalSpeed;
            originalSpeeds.Remove(agent);

        }
    }
}
