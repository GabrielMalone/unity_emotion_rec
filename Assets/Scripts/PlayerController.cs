using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using GSM = GameStateManagerScript;

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
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        transform.rotation = Quaternion.identity;

    }
    // --------------------------------------------------------------------------------------------
    void FixedUpdate() 
    {
        GamePadControls();
    }
    // --------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

        RecordKeyStrokes();

        if (GSM.Instance.playerControllerDisabled) return;

        if (Mouse.current.leftButton.isPressed)
        {
            // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            // Debug.Log("Mouse Pos: " + mousePos);
            // // mouse position - player object position
            // Vector2 direction = mousePos - transform.position;
            // // set the player game object facing that direction
            // transform.up = direction.normalized;
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
    // --------------------------------------------------------------------------------------------
    void SpeedCheck(Rigidbody2D rb)
    {
        if (rb.linearVelocity.magnitude > maxPlayerSpeed)
        {
            // normalized takes away speed and gives only direction. 
            rb.linearVelocity = rb.linearVelocity.normalized * maxPlayerSpeed;
        }
    }
    // --------------------------------------------------------------------------------------------
    void GamePadControls()
    {

        if (GSM.Instance.playerControllerDisabled) return;

        if (Gamepad.current == null)
            return;

        Gamepad gamepad = Gamepad.current;
        // left stick
        Vector2 leftStick = gamepad.leftStick.ReadValue();
        if (leftStick.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (leftStick.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        rb.AddForce(leftStick * thrustForce);
        // gace buttons
        if (gamepad.buttonSouth.wasPressedThisFrame)
            Debug.Log("South button pressed"); 

        if (gamepad.buttonEast.wasPressedThisFrame)
            Debug.Log("East button pressed"); 
        // bumpers
        if (gamepad.leftShoulder.wasPressedThisFrame)
            Debug.Log("Left bumper");

        if (gamepad.rightShoulder.wasPressedThisFrame)
            Debug.Log("Right bumper");
    }
    // --------------------------------------------------------------------------------------------
    void RecordKeyStrokes()
    {
        // telemetry data
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (key.wasPressedThisFrame)
                {
                    GSM.Instance.keyStrokes.Add(
                        new GSM.KeyStroke
                        {
                            key = key.name,
                            time = Time.time
                        }
                    );
                }
            }
        }
    }
    // --------------------------------------------------------------------------------------------

}
