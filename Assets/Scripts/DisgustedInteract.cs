using UnityEngine;
using UnityEngine.UI;

public class DisgustedInteract : MonoBehaviour
{
    public GameObject player;
    private bool playerInRange = false;
    [Header("Disgusted Intro Speech")]
    public Text dialogueText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            TalkToPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is near!");
            playerInRange = true;
        }
        
    }

    public void TalkToPlayer()
    {
        dialogueText.enabled = true;
    }
    


}
