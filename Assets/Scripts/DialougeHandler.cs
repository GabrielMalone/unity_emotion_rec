using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Linq;

public class DialougeHandler : MonoBehaviour
{
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
        //nextText(0); // starts the first line
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

        if (dialouge.Dialouge[index].expression != null) //if an sprite isn't used for text, than it uses the last sprite used
        {
            face.sprite = dialouge.Dialouge[index].expression;
        }

        if (correct) textBox.text = dialouge.Dialouge[index].text;
        else
        {
            isQuestioning = false;
            textBox.text = dialouge.Dialouge[index].wrongText;
        }

        charactersSpoken = 0;
        textBox.maxVisibleCharacters = 0;

        // for questions
        if (dialouge.Dialouge[index].isQuestion && correct)
        {
            options.gameObject.SetActive(true);
            isQuestioning = true;

            int[] randomOrder = {0, 1, 2, 3};
            if (Random.value > 0.5) randomOrder.Reverse();

            for (int i = 0; i < answers.Length; i++)
            {
                answers[i].text = dialouge.Dialouge[index].Answers[randomOrder[i]];

                if (randomOrder[i] == 0) correctOption = i;
            }
        }
        else if (correct)
        {
            options.gameObject.SetActive(false);

            dialougeIndex++;
        }
    }

    public void SubmitAnswer(int optionPicked)
    {
        if (optionPicked == correctOption)
        {
            dialougeIndex++;
            isQuestioning = false;
            nextText(dialougeIndex);
        }
        else
        {
            nextText(dialougeIndex, false);
        }
    }

    public void end()
    {
        dialouge = null;
    }
}
