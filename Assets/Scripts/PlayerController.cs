using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float thrustForce = 1f;
    public float torque = 0.5f;
    public float maxPlayerSpeed = 20f;
    public float jumpforce = 10f; // how much in the up direction
    public float boostMultiplier = 2f; // speed boost on jump
    public float boostDuration = 0.5f;
    public Transform playerSprite;
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Debug.Log("Mouse Pos: " + mousePos);
            // mouse position - player object position
            Vector2 direction = mousePos - transform.position;
            // set the player game object facing that direction
            transform.up = direction.normalized;
        }


        // Forward
        if (Keyboard.current.wKey.isPressed)
        {
            rb.AddForce(transform.up * thrustForce);
        }

        // Strafe left
        if (Keyboard.current.aKey.isPressed)
        {
            rb.AddForce(-transform.right * thrustForce);
            spriteRenderer.flipX = false;
        }

        // Backward
        if (Keyboard.current.sKey.isPressed)
        {
            rb.AddForce(-transform.up * thrustForce);
        }

        // Strafe right
        if (Keyboard.current.dKey.isPressed)
        {
            rb.AddForce(transform.right * thrustForce);
            spriteRenderer.flipX = true;
        }

        SpeedCheck(rb);

    }



    void SpeedCheck(Rigidbody2D rb)
    {
        if (rb.linearVelocity.magnitude > maxPlayerSpeed)
        {
            // normalized takes away speed and gives only direction. 
            rb.linearVelocity = rb.linearVelocity.normalized * maxPlayerSpeed;
        }
    }

}
