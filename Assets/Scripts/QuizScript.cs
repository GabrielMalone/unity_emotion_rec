using UnityEngine;
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
    public Image face;
    public TextMeshProUGUI textBox;
    bool isSpeaking; //is dialouge currently being written on screen
    float charactersSpoken;
    [Header("Sound FX")]
    public AudioSource audioSource;
    public AudioClip incorrectSFX;
    public AudioClip correctSFX;
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        SetEmotionButtons();
    }
    // --------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
        ScrollText();
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
            TextMeshProUGUI TMP = emotionButton.GetComponentInChildren<TextMeshProUGUI>();
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
        // check gameStateManager for current emotion.
        // compare against buttons in this quiz
        if (buttonOptions[optionPicked] == GSM.Instance.currentEmotion)
        {
            audioSource.PlayOneShot(correctSFX);
            GSM.Instance.playerAnsweredCorrectly = true;
            // resets needed for next interaction
            GSM.Instance.PickNextEmotion();
            GSM.Instance.SetEmotionVideo();
            GSM.Instance.agreedToHelp = false;
        }
        // what do when wrong
        {
            textBox.text = 
                GSM.Instance.incorrectResponses
                [Random.Range(0, GSM.Instance.incorrectResponses.Length)];
            // play sound
            audioSource.PlayOneShot(incorrectSFX);
            // rewrite text in main dialogue area
        }
    }
}
    