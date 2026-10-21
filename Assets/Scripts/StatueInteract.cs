using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatueInteract : MonoBehaviour
{
    public GameObject player;
    public CameraFollow cameraFollow;
    private bool playerInRange = false;

    [Header("Radius Settings")]
    public float interactionRadius = 3f;
    private bool playerLocked = false;
    private float timeLocked;
    private float timeLockDelay = 2f;
    private float ogLinearDamp;

    [Header("dialouge")]
    public DialougeObject dialouge;


    private Rigidbody2D rb;
    private bool agreedToWatch = false;


    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
        ogLinearDamp = rb.linearDamping;
    }

    void Update()
    {
        UnlockPlayer();
    }

    IEnumerator WaitForAgreement()
    {

        // will needs to base this on gamestatemanager deciding
        // which statue is talking 

        GameStateManagerScript.dialougeHandler.startDialouge(dialouge);

        yield return new WaitUntil(() => GameStateManagerScript.dialougeHandler.dialougeEnded);

        //if (GameStateManagerScript.dialougeHandler.lastAnswered == 1)
        //{
            agreedToWatch = true;
            GameStateManagerScript.screenFader.FadeToBlack();
        //}
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            LockPlayer();
            cameraFollow.ZoomIn();
            StartCoroutine(WaitForAgreement());
            // start dialogue interaction here, when okay hit
            // proceed to nxt part
            // so guess need a while loop
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            cameraFollow.ZoomOut();
        }
    }

    void LockPlayer()
    {
        if (rb!=null)
        {
            Debug.Log($"LOCKING PLAYER! with og damp of {ogLinearDamp}");
            StartCoroutine(SlowDownSmoothly());
            playerLocked = true;
            timeLocked = Time.time;
        }
    }

    void UnlockPlayer()
    {
        if (!playerLocked) return;
        
        if (rb != null)
        {
            if (Time.time - timeLocked > timeLockDelay)
            {
                rb.linearDamping = ogLinearDamp;
                playerLocked = false;
            }

        }
    }
    
    private IEnumerator SlowDownSmoothly()
    {
        Vector2 initialSpeed = rb.linearVelocity;
        float initialAngular = rb.angularVelocity; 
        float elapsedTime = 0f;
        float slowDuration = 0.5f;

        while (elapsedTime < slowDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slowDuration;
            rb.linearVelocity = Vector2.Lerp(initialSpeed, Vector2.zero, t);
            rb.angularVelocity = Mathf.Lerp(initialAngular, 0f, t);
            
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.linearDamping = 200f; 
    }
}