using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows.Speech;

public class DialougeHandler : MonoBehaviour
{

    [Header("Dialogue Setup")]

    public float textSpeed; // the length in seconds every letter comes on screen
    public DialougeObject dialouge;

    public Image face;
    public VideoPlayer videoFace;
    public TextMeshProUGUI textBox;

    public Transform options;
    public TextMeshProUGUI[] answers;

    public Animator animator;

    bool isSpeaking; //is dialouge currently being written on screen
    int dialougeIndex; //the place you are in the dialouge

    int correctOption;

    bool isQuestioning;

    float charactersSpoken;

    public bool dialougeEnded;

    public int lastAnswered;

    bool randomizeAnswers;

    //GameObject menu;

    void Start()
    {
     //   menu = transform.GetChild(0).gameObject;
    }

    public void startDialouge(DialougeObject newDialouge)
    {
        if (dialouge == null)
        {
            dialouge = newDialouge;
            dialougeEnded = false;

            animator.Play("pull up");
            dialougeIndex = 0;
            nextText(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //advancing the text per character
        if (isSpeaking) charactersSpoken += Time.deltaTime * textSpeed;

        textBox.maxVisibleCharacters = (int)charactersSpoken;


        if (Keyboard.current.eKey.wasPressedThisFrame && !dialougeEnded)
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
            dialougeEnded = true;
            randomizeAnswers = false;
            isQuestioning = false;
            return;
        }

        DialougeObject.Speech currentDialouge = dialouge.Dialouge[index];

        currentDialouge.text = changeTextToHaveVaribles(currentDialouge.text);
        currentDialouge.wrongText = changeTextToHaveVaribles(currentDialouge.wrongText);

        videoFace.gameObject.SetActive(false);
        face.gameObject.SetActive(true);

        if (currentDialouge.videoExpression != null)//if a video isn't used, than it wont show up and will used last sprite used
        {
            videoFace.clip = dialouge.Dialouge[index].videoExpression;
            face.gameObject.SetActive(false);
            videoFace.gameObject.SetActive(true);
            videoFace.Play();
        }
        else if (currentDialouge.expression != null) //if an sprite isn't used for text, than it uses the last sprite used
        {
            face.sprite = currentDialouge.expression;
        }

        if (correct)
        {
            textBox.text = currentDialouge.text;
        }
        else
        {
            textBox.text = currentDialouge.wrongText;
        }

        charactersSpoken = 0;
        textBox.maxVisibleCharacters = 0;

        // for questions
        int amountOfAnswers = currentDialouge.answers.Length;
        if (amountOfAnswers > 0 && correct)
        {
            //options.gameObject.SetActive(true);
            isQuestioning = true;
            randomizeAnswers = true;

            for (int i = 0; i < options.childCount; i++)
            {
                answers[i].transform.parent.gameObject.SetActive(false);
            }


            int[] randomOrder = new int[amountOfAnswers];
            int randomStartingPoint = Random.Range(0, amountOfAnswers);
            int randomSteps = Random.Range(1, amountOfAnswers);

            if (currentDialouge.noWrongAnswers)
            {
                randomStartingPoint = 0;
                randomSteps = 1;
            }

            for (int i = 0; i < amountOfAnswers; i++)
            {
                int indexPointer = (randomStartingPoint + (i * randomSteps)) % amountOfAnswers;
                if (i != 0 && indexPointer == randomStartingPoint)
                {
                    indexPointer = (indexPointer + 1) % amountOfAnswers;
                    randomStartingPoint = (randomStartingPoint + 1) % amountOfAnswers;
                }
                randomOrder[i] = indexPointer;
            }
            //if (Random.value > 0.5) randomOrder.Reverse();

            for (int i = 0; i < amountOfAnswers; i++)
            {
                answers[i].transform.parent.gameObject.SetActive(true);
                answers[i].text = currentDialouge.answers[randomOrder[i]];

                if (randomOrder[i] == 0) correctOption = i;
            }
        }
        else if (correct && amountOfAnswers == 0)
        {
            randomizeAnswers = false;
            for (int i = 0; i < options.childCount; i++)
            {
                answers[i].transform.parent.gameObject.SetActive(false);
            }

            dialougeIndex++;
        }
    }

    string changeTextToHaveVaribles(string text) //used to state player name or any other varible
    {
        text = text.Replace("{playerName}", GameStateManagerScript.playerName);
        return text;
    }

    public void SubmitAnswer(int optionPicked)
    {
        lastAnswered = optionPicked;

        DialougeObject.Speech current = dialouge.Dialouge[dialougeIndex];

        if (optionPicked == correctOption || current.noWrongAnswers) {
            isQuestioning = false;
            dialougeIndex++;

            if (current.answerPaths.Length > optionPicked && current.answerPaths[optionPicked] != null)
            {
                dialouge = current.answerPaths[optionPicked];
                dialougeIndex = 0;
            }

            nextText(dialougeIndex, true);
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
