using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody2D rb;

    public GameObject GameStateManger;
    private ScreenFader  screenFader;


    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
        ogLinearDamp = rb.linearDamping;
        screenFader = GameStateManger.GetComponent<ScreenFader>();
    }


    void Update()
    {
        UnlockPlayer();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            LockPlayer();
            cameraFollow.ZoomIn();
            screenFader.FadeToBlack();
      
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            cameraFollow.ZoomOut();
            screenFader.FadeFromBlack();
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
                Debug.Log($"UNLOCKING PLAYER! with damp of {ogLinearDamp}");
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

    private void FadeScreenToBlack()
    {
        
    }




}