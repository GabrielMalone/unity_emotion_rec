using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GSM = GameStateManagerScript;

public class StatueInteract : MonoBehaviour
{
    public GameObject player;
    public CameraFollow cameraFollow;
    private bool playerInRange = false;

    [Header("Radius Settings")]
    public float interactionRadius = 3f;
    private float timeLocked;
    private float timeLockDelay = 2f;
    private float ogLinearDamp;
    private Rigidbody2D rb;
    public GameObject GameStateManger;
    public GameObject face;
    private ScreenFader screenFader;
    private GameObject Quiz;
    private TextMeshProUGUI quizMainText;
    private GameObject DialogueHolder;
    private DialougeHandler dialougeHandler;
    private TextMeshProUGUI mainText;
    private bool agreedToWatch = false;
    private bool hasRun = false;
    private SpriteRenderer statueSprite;


    void Start()
    {
        InitSequence();
    }

    void Update()
    {
        StartQuiz();
    }

    void FixedUpdate()
    {
        if (GSM.Instance.playerAnsweredCorrectly && playerInRange)
        {
            CorrectAnswerSequence();
        }
    }


    void InitSequence()
    {
        // slow player
        rb = player.GetComponent<Rigidbody2D>();
        ogLinearDamp = rb.linearDamping;

        // intro dialogue stuff
        screenFader = GameStateManger.GetComponent<ScreenFader>();
        DialogueHolder = screenFader.DialogueHolder;
        mainText = DialogueHolder.GetComponentInChildren<TextMeshProUGUI>(true);
        dialougeHandler = DialogueHolder.GetComponent<DialougeHandler>();

        // quiz stuff
        Quiz = screenFader.Qiuz;
        quizMainText = Quiz.GetComponentInChildren<TextMeshProUGUI>(true);

        // what do i look like
        statueSprite = GetComponent<SpriteRenderer>();
    }

    void CorrectAnswerSequence()
    {
        // play some effect
        Debug.Log("PLAYER ANSWERED CORRECLTY!");
        GSM.Instance.playerAnsweredCorrectly = false;
        screenFader.readyForQuiz = false;
        // destroy statue
        Quiz.SetActive(false);
        // let them go
        UnlockPlayer();
        Debug.Log($"Destroying statue: {gameObject.name}");
        Destroy(gameObject);
    }

    void IntroSpeech()
    {
        // will needs to base this on 
        // gamestatemanager deciding
        // which statue is talking 
        mainText.text = 
            $"hello {GSM.Instance.playerName}! Can you help me figure out what my friend is feeling?";
    }

    public void StartQuiz()
    {
        if (screenFader.readyForQuiz && !hasRun)
        {
            Quiz.SetActive(true);
            quizMainText.text = 
                $"{GSM.Instance.playerName}, which emotion do you think they were feeling?";
            // just do StartQuiz once
            hasRun = true;
        }
        // reset when done w statue
        if (GSM.Instance.playerAnsweredCorrectly)
        {
            hasRun = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            LockPlayer();
            cameraFollow.ZoomIn();
            // set my image as the dialogue UI image
            Image statue_face = face.GetComponent<Image>();
            statue_face.sprite = statueSprite.sprite;
            DialogueHolder.SetActive(true);
            IntroSpeech();
            // start dialogue interaction here
            StartCoroutine(WaitForAgreement());
        }
    }

    IEnumerator WaitForAgreement()
    {
        while (!agreedToWatch)
        {
            if (Keyboard.current.eKey.isPressed || GSM.Instance.agreedToHelp)
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
        GSM.Instance.playerControllerDisabled = true;
        Debug.Log($"controller disabled? {GSM.Instance.playerControllerDisabled}");
        if (rb!=null && playerInRange)
        {
            Debug.Log($"LOCKING PLAYER! with og damp of {ogLinearDamp}");
            StartCoroutine(SlowDownSmoothly());
            timeLocked = Time.time;
        }
    }

    void UnlockPlayer()
    {
        if (!playerInRange) return;
        GSM.Instance.playerControllerDisabled = false;

        if (rb != null)
        {
            if (Time.time - timeLocked > timeLockDelay)
            {
                rb.linearDamping = ogLinearDamp;
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