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
    private Rigidbody2D rb;

    public GameObject GameStateManger;
    private ScreenFader  screenFader;
    private GameObject DialogueHolder;
    private DialougeHandler dialougeHandler;
    private TextMeshProUGUI mainText;
    private bool agreedToWatch = false;


    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
        ogLinearDamp = rb.linearDamping;
        screenFader = GameStateManger.GetComponent<ScreenFader>();
        DialogueHolder = screenFader.DialogueHolder;
        mainText = DialogueHolder.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    void Update()
    {
        UnlockPlayer();
    }

    void IntroSpeech()
    {
        mainText.text = "hello";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            LockPlayer();
            cameraFollow.ZoomIn();
            DialogueHolder.SetActive(true);
            IntroSpeech();
            // start dialogue interaction here, when okay hit
            // proceed to nxt part
            // so guess need a while loop
            StartCoroutine(WaitForAgreement());
        }
    }

    IEnumerator WaitForAgreement()
    {
        while (!agreedToWatch)
        {
            if (Keyboard.current.eKey.isPressed)
            {
                agreedToWatch = true;
            }
            yield return null;
        }
        screenFader.FadeToBlack();
        DialogueHolder.SetActive(false);
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

    private void FadeScreenToBlack()
    {
        
    }
}