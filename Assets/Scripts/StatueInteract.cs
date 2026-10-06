using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StatueInteract : MonoBehaviour
{
    public GameObject player;
    public CameraFollow cameraFollow;
    private bool playerInRange = false;

    [Header("Statue Intro Speech")]
    public Text dialogueText;
    [TextArea(2, 5)]
    public string message = "im disgusted";

    [Header("Radius Settings")]
    public float interactionRadius = 3f;

    [Header("Typewriter Settings")]
    public float charactersPerSecond = 30f;

    [Header("Text Position")]
    public Vector3 textWorldOffset = new Vector3(0f, 1.5f, 0f);
    private Coroutine typewriterRoutine;

    public DialougeHandler dialougeHandler;
    public DialougeObject TheConversation;

    private bool playerLocked = false;
    private float timeLocked;
    private float timeLockDelay = 2f;
    private float ogLinearDamp;
    private Rigidbody2D rb;


    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.color = Color.green;
        }

        rb = player.GetComponent<Rigidbody2D>();
        ogLinearDamp = rb.linearDamping;
    }

    void LateUpdate()
    {
        if (dialogueText == null || Camera.main == null) return;
        // Keep the text positioned above the statue every frame
        Vector3 worldPos = transform.position + textWorldOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        dialogueText.rectTransform.position = screenPos;
    }

    void Update()
    {
        UnlockPlayer();
    }

    public void TalkToPlayer()
    {
        if (dialogueText == null) return;

        dialogueText.enabled = true;

        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
        }
        typewriterRoutine = StartCoroutine(TypewriterEffect(message));
    }

    private void StopTalking()
    {
        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;
        }
        if (dialogueText != null)
        {
            dialogueText.enabled = false;
            dialogueText.text = "";
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            TalkToPlayer();
            LockPlayer();
            cameraFollow.ZoomIn();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopTalking();
            cameraFollow.ZoomOut();
        }
    }


    void LockPlayer()
    {
        if (rb!=null)
        {
            Debug.Log($"LOCKING PLAYER! with og damp of {ogLinearDamp}");
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.linearDamping = 20f;
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


    private IEnumerator TypewriterEffect(string fullText)
    {
        dialogueText.text = "";
        float delay = 1f / Mathf.Max(charactersPerSecond, 0.01f);

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
    }

    // Optional: visualize the radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}