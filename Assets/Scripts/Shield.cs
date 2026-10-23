using UnityEngine;

public class Shield : MonoBehaviour
{
    private Rigidbody2D rb;
    public float radius = 2f;
    public float shieldForce = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FixedUpdate()
    {
        GetObjectsInRadius();
    }


    void GetObjectsInRadius()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(rb.position, radius);

        foreach (Collider2D obj in objects)
        {
            if (obj.gameObject.CompareTag("Firefly"))
            {
                // normalize so all receive same force regardless of distance 
                Vector2 direction = (obj.transform.position - gameObject.transform.position).normalized;
                obj.attachedRigidbody.AddForce(shieldForce * direction, ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
