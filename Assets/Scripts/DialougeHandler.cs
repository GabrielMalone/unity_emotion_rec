using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using GSM = GameStateManagerScript;

public class DialougeHandler : MonoBehaviour
{

    [Header("Dialogue Setup")]
    public GameObject GameStateManager;
    public GameObject mainTextObj;
    public float textSpeed; // the length in seconds every letter comes on screen
    public DialougeObject dialouge;  
    public Image face;
    public TextMeshProUGUI textBox;
    public Transform options;
    public TextMeshProUGUI[] answers;
    public Animator animator;
    bool isSpeaking; //is dialouge currently being written on screen
    int dialougeIndex; //the place you are in the dialouge
    int correctOption;
    bool isQuestioning;
    float charactersSpoken;

    public void StartUpDialouge(DialougeObject newDialouge)
    {
        if (dialouge == null)
        {
            dialouge = newDialouge;
            animator.Play("pull up");
            dialougeIndex = 0;
            nextText(0);
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //advancing the text per character
        if (isSpeaking) charactersSpoken += Time.deltaTime * textSpeed;

        textBox.maxVisibleCharacters = (int)charactersSpoken;

        if (Keyboard.current.eKey.wasPressedThisFrame && !isQuestioning && dialouge != null)
        {
            if (isSpeaking)
            {
                charactersSpoken = 10000; //automatically skips the text showing up slowly if pressed during
            }
            else
            {
                nextText(dialougeIndex);
            }
        }

        isSpeaking = textBox.maxVisibleCharacters < textBox.text.Length; //the statue is speaking if it is currently writting text to the screen
    }

    void nextText(int index, bool correct = true)
    {
        if (dialouge.Dialouge.Length <= index)
        {
            animator.Play("pull down");
            return;
        }
    }

    public void SubmitAnswer(int optionPicked)
    {
        if (optionPicked == 0){
            GSM.Instance.agreedToHelp = true;
        }
    }

    public void end()
    {
        dialouge = null;
    }
}
