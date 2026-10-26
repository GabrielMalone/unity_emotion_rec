using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using GSM = GameStateManagerScript;
using System.Collections.Generic;

public class QuizScript : MonoBehaviour
{
    // --------------------------------------------------------------------------------------------
    [Header("Dialogue Setup")]
    public GameObject mainTextObj;
    public float textSpeed; // the length in seconds every letter comes on screen

    [Header("Emotion Options")]
    public GameObject[] emotionOptions;
    Dictionary<int, string> buttonOptions = new Dictionary<int, string>();
    private Image face;
    public TextMeshProUGUI textBox;
    bool isSpeaking; //is dialouge currently being written on screen
    float charactersSpoken;

    [Header("Replay Option")]
    public GameObject replayButton;

    [Header("Sound FX")]
    public AudioSource audioSource;
    public AudioClip incorrectSFX;
    public AudioClip correctSFX;
    private bool quizStarted = false;

    [Header("Associated Statue")]
    public GameObject myStatue;
    private Transform statuePos;
    // --------------------------------------------------------------------------------------------
    void Start()
    {

    }
    // --------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        ScrollText();
        
        // this is only happening 
        // once per game
        // check for null on associate statue as well
        // so quiz does not restart after quiz end
        if (!quizStarted && myStatue)
        {
            quizStarted = true;
            statuePos = myStatue.transform;
            SetEmotionButtons();
            // telemetry - when did quiz start
            GSM.Instance.result.timeQuizStarted = Time.time;
            // current emotion being tested
            GSM.Instance.result.emotionName = GSM.Instance.currentEmotion;
            // what question is the player on
            GSM.Instance.result.questionNumber = GSM.Instance.numQuestionsCompleted + 1;
        }
    }
    // --------------------------------------------------------------------------------------------
    void ScrollText()
    {
        //advancing the text per character
        if (isSpeaking) charactersSpoken += Time.deltaTime * textSpeed;
        textBox.maxVisibleCharacters = (int)charactersSpoken;
        //the statue is speaking if it is currently writting text to the screen 
        isSpeaking = textBox.maxVisibleCharacters < textBox.text.Length;
    }
    // --------------------------------------------------------------------------------------------
    public void SetEmotionButtons()
    {
        // let's randomize it, though. 
        HashSet<string> seen = new HashSet<string>();
        int option = 0;

        // handle reset button
        if (!GSM.Instance.replaySelected)
        {
            replayButton.SetActive(true);
        }

        // bool firstButtonSet = false;

        foreach (GameObject emotionButton in emotionOptions)
        {
            TextMeshProUGUI TMP = 
                emotionButton.GetComponentInChildren<TextMeshProUGUI>();

            bool validCandidateFound = false;

            while (!validCandidateFound)
            {
                int idx = Random.Range(0, GSM.Instance.emotionOptions.Length);
                TMP.text = GSM.Instance.emotionOptions[idx];
                if (!seen.Contains(TMP.text))
                {
                    seen.Add(TMP.text);
                    // track which button has which emotion
                    buttonOptions[option] = TMP.text;
                    validCandidateFound = true;
                }
            }
            // // autoselect first button for gamepad
            // if (!firstButtonSet)
            // {
            //     EventSystem.current.SetSelectedGameObject(emotionButton);
            //     firstButtonSet = true;
            // }
            option ++;
        }
    }
    // --------------------------------------------------------------------------------------------
    public void CorrectResponse()
    {
        audioSource.PlayOneShot(correctSFX);
        GSM.Instance.result.wasCorrect = true;
        GSM.Instance.ConfettiA.transform.position = statuePos.position;
        GSM.Instance.ConfettiB.transform.position = statuePos.transform.position;
        GSM.Instance.ConfettiA.Play();
        GSM.Instance.ConfettiB.Play();
    }
    // --------------------------------------------------------------------------------------------
    public void IncorrectResponse()
    {
        // get random wrong resposne
        textBox.text =
            GSM.Instance.incorrectResponses
            [Random.Range(0, GSM.Instance.incorrectResponses.Length)];
        // play sound
        audioSource.PlayOneShot(incorrectSFX);
        GSM.Instance.result.wasCorrect = false;
        GSM.Instance.Smoke.transform.position = statuePos.position;
        GSM.Instance.Smoke.Play();
    }
    // --------------------------------------------------------------------------------------------
    public void ReplaySelected()
    {
        Debug.Log("REPLAY SELECTED");
        GSM.Instance.replaySelected = true;
        GSM.Instance.result.replayUsed = true;
        replayButton.SetActive(false);
    }
    // --------------------------------------------------------------------------------------------
    public void SubmitAnswer(int optionPicked)
    {
        // if replay button selected
        if (optionPicked == 4)
        {
            ReplaySelected();
            return;
        }
        quizStarted = false;
        GSM.Instance.agreedToHelp = false;
        GSM.Instance.numQuestionsCompleted ++;
        // telemetry stuff
        GSM.EmotionGuess emotionGuess = new GSM.EmotionGuess();
        emotionGuess.emotion = buttonOptions[optionPicked];
        emotionGuess.time = Time.time;
        GSM.Instance.result.emotion_guessed = emotionGuess;
        GSM.Instance.result.playerName = GSM.Instance.playerName;
        GSM.Instance.result.timeGameStarted = GSM.Instance.timeGameStarted;
        // when did player finish this quiz
        GSM.Instance.result.timeQuizCompleted = Time.time;
        // check gameStateManager for current emotion.
        // compare against buttons in this quiz
        if (buttonOptions[optionPicked] == GSM.Instance.currentEmotion)
        {
            CorrectResponse();
        }
        // what do when wrong
        else
        {
            IncorrectResponse();
        }
        // save telemetry to disk
        GSM.Instance.results.Add(GSM.Instance.result);
        GSM.Instance.result = new GSM.EmotionResult();
        GSM.Instance.SaveResults();
        GSM.Instance.playerAnswered = true;
    }
}
    