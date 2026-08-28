using UnityEngine;

public class TestMovment : MonoBehaviour
{
    public float speed;
    public float maxSpeed;
    private Rigidbody2D rigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movmentDir = new Vector2(x, y);
        rigidbody.AddForce(movmentDir * speed, ForceMode2D.Impulse);
        rigidbody.linearVelocity = Vector2.ClampMagnitude(rigidbody.linearVelocity, maxSpeed);

        if (x == 0 && y == 0)
        {
            rigidbody.linearVelocity = Vector2.zero;
        }

    }
}
