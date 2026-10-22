using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GSM = GameStateManagerScript;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class QuizScript : MonoBehaviour
{
    // --------------------------------------------------------------------------------------------
    [Header("Dialogue Setup")]
    public GameObject mainTextObj;
    public float textSpeed; // the length in seconds every letter comes on screen
    [Header("Emotion Options")]
    public GameObject[] emotionOptions;
    Dictionary<int, string> buttonOptions = new Dictionary<int, string>();
    public Image face;
    public TextMeshProUGUI textBox;
    bool isSpeaking; //is dialouge currently being written on screen
    float charactersSpoken;
    [Header("Sound FX")]
    public AudioSource audioSource;
    public AudioClip incorrectSFX;
    public AudioClip correctSFX;
    private bool quizStarted = false;
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

        if (!quizStarted)
        {
            quizStarted = true;
            SetEmotionButtons();
            // telemetry - when did quiz start
            GSM.Instance.result.timeStarted = Time.time;
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
            option++;
        }
    }
    // --------------------------------------------------------------------------------------------
    public void SubmitAnswer(int optionPicked)
    {
        GSM.EmotionGuess emotionGuess = new GSM.EmotionGuess();
        emotionGuess.emotion = buttonOptions[optionPicked];
        emotionGuess.time = Time.time;
        GSM.Instance.result.emotionsGuessed.Add(emotionGuess);
        // check gameStateManager for current emotion.
        // compare against buttons in this quiz
        if (buttonOptions[optionPicked] == GSM.Instance.currentEmotion)
        {
            audioSource.PlayOneShot(correctSFX);
            // telemetry data
            GSM.Instance.result.playerName = GSM.Instance.playerName;
            // when did player finish this quiz
            GSM.Instance.result.timeCompleted = Time.time;
            // increment questions completed
            GSM.Instance.numQuestionsCompleted++;
            GSM.Instance.playerAnsweredCorrectly = true;
            quizStarted = false;     

            // resets needed for next interaction
            GSM.Instance.PickNextEmotion();
            GSM.Instance.SetEmotionVideo();
            GSM.Instance.agreedToHelp = false;

            // should save data to list then 
            // create new instance of result here. 
            GSM.Instance.results.Add(GSM.Instance.result);
            GSM.Instance.result = new GSM.EmotionResult();
        }
        // what do when wrong
        else
        {
            // get random wrong resposne
            textBox.text = 
                GSM.Instance.incorrectResponses
                [Random.Range(0, GSM.Instance.incorrectResponses.Length)];
            // play sound
            audioSource.PlayOneShot(incorrectSFX);
        }
        // save telemetry to disk
        GSM.Instance.SaveResults();
    }
}
    