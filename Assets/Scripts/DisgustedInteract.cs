using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class DisgustedInteract : MonoBehaviour
{
    public GameObject player;
    private bool playerInRange = false;
 
    [Header("Disgusted Intro Speech")]
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
    private bool hasTalked = false;
 
    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.color = Color.green;
        }
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
        if (player == null) return;
 
        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool inRangeNow = distance <= interactionRadius;
 
        if (inRangeNow && !playerInRange)
        {
            // just entered range
            playerInRange = true;
            TalkToPlayer();
        }
        else if (!inRangeNow && playerInRange)
        {
            // just left range
            playerInRange = false;
            StopTalking();
        }
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