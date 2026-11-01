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
        // some algo testing
        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.pKey.wasPressedThisFrame)
        {
            GSM.Instance.lvl1 = false;
            GSM.Instance.numQuestionsCompleted = 0;
            GSM.Instance.happySeen = 0;
            GSM.Instance.sadSeen = 0;
            GSM.Instance.afraidSeen = 0;
            GSM.Instance.angrySeen = 0;
            for (int i = 0; i < 1000; i++)
            {
                GSM.Instance.SetNextRandomEmotion();
                GSM.Instance.numQuestionsCompleted ++ ;
            }
        }

        if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetPlayerPosition();
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
    public void ResetPlayerPosition()
    {
        if (GSM.Instance.lvl1)
        {
            transform.position = GSM.Instance.lvl1SpawnPoint.position;
        }
        if (GSM.Instance.lvl2)
        {
            transform.position = GSM.Instance.lvl2spawnPoint.position;
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
