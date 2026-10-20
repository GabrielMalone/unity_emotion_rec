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
    private GameStateManagerScript gsms;
    private ScreenFader  screenFader;
    private GameObject Quiz;
    private TextMeshProUGUI quizMainText;
    private DialougeHandler quizDialogueHandler;
    private GameObject DialogueHolder;
    private DialougeHandler dialougeHandler;
    private TextMeshProUGUI mainText;
    private bool agreedToWatch = false;
    private bool readyForQuiz = false;


    void Start()
    {
        // slow player
        rb = player.GetComponent<Rigidbody2D>();
        ogLinearDamp = rb.linearDamping;

        // intro dialogue stuff
        screenFader = GameStateManger.GetComponent<ScreenFader>();
        DialogueHolder = screenFader.DialogueHolder;
        mainText = DialogueHolder.GetComponentInChildren<TextMeshProUGUI>(true);
        dialougeHandler = DialogueHolder.GetComponent<DialougeHandler>();
        gsms = GameStateManger.GetComponent<GameStateManagerScript>();

        // quiz stuff
        Quiz = screenFader.Qiuz;
        quizMainText = Quiz.GetComponentInChildren<TextMeshProUGUI>(true);

    }

    void Update()
    {
        StartQuiz();
    }

    void IntroSpeech()
    {
        // will needs to base this on 
        // gamestatemanager deciding
        // which statue is talking 
        mainText.text = 
            $"hello {gsms.playerName}! Can you help me figure out what my friend is feeling?";
    }

    public void StartQuiz()
    {
        if (screenFader.readyForQuiz)
        {
            Quiz.SetActive(true);
            quizMainText.text = 
                $"{gsms.playerName} Which emotion do you think they were feeling?";
            // eventually
            UnlockPlayer();
        }
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
            if (Keyboard.current.eKey.isPressed || dialougeHandler.agreedToWatch)
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
        gsms.playerControllerDisabled = true;
        Debug.Log($"controller disabled? {gsms.playerControllerDisabled}");
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
        gsms.playerControllerDisabled = false;
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