using TMPro;
using UnityEngine;
using GSM = GameStateManagerScript;

public class OutroDialogueScipt : MonoBehaviour
{

    public TextMeshProUGUI buttonText;
    private bool textSet = false;
    // --------------------------------------------------------------------------------------------
    public void Start()
    {

    }
    // --------------------------------------------------------------------------------------------
    public void Update()
    {     
        if (GSM.Instance.playerAnswered && 
            GSM.Instance.result.emotion_guessed.emotion == 
            GSM.Instance.currentEmotion
        )
        {
            buttonText.text = $"{GSM.Instance.currentEmotion} is Correct! ";
        }

        if (GSM.Instance.playerAnswered && 
            GSM.Instance.result.emotion_guessed.emotion
            != GSM.Instance.currentEmotion
        )
        {
            buttonText.text = $" Almost! They were {GSM.Instance.currentEmotion} ";
        }

        buttonText.text += "\n\n\nClick Here to Continue";
    }
    // --------------------------------------------------------------------------------------------
    public void OutroHandler()
    {
        GSM.Instance.readyToMoveOn = true;
    }

}
