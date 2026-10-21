using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizScript : MonoBehaviour
{

    [Header("Dialogue Setup")]
    public GameObject GameStateManager;
    private GameStateManagerScript gsm;
    public GameObject mainTextObj;

    public float textSpeed; // the length in seconds every letter comes on screen
    public DialougeObject dialouge;

    [Header("Emotion Options")]
    public GameObject [] emotionOptions;

    public Image face;
    public TextMeshProUGUI textBox;

    bool isSpeaking; //is dialouge currently being written on screen

    float charactersSpoken;

    void Start()
    {
        gsm = GameStateManager.GetComponent<GameStateManagerScript>();
        SetEmotionButtons();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollText();
    }

    void ScrollText()
    {
        //advancing the text per character
        if (isSpeaking) charactersSpoken += Time.deltaTime * textSpeed;
        textBox.maxVisibleCharacters = (int)charactersSpoken;
        //the statue is speaking if it is currently writting text to the screen 
        isSpeaking = textBox.maxVisibleCharacters < textBox.text.Length;    
    }

    public void SetEmotionButtons()
    {
        int idx = 0;
        foreach(GameObject emotionButton in emotionOptions)
        {
            TextMeshProUGUI TMP = emotionButton.GetComponentInChildren<TextMeshProUGUI>();
            TMP.text = gsm.introAreaEmotionOptions[idx];
            idx ++ ;
        }
    }

    public void SubmitAnswer(int optionPicked)
    {
        // check gameStateManager for current emotion.
        if (optionPicked == 0)
        {
            gsm.playerAnsweredCorrectly = true;
        }
    }
}
