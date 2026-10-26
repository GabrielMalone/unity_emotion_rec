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
            buttonText.text = $"Good job, {GSM.Instance.playerName}! {GSM.Instance.currentEmotion} is Correct! ";
        }

        if (GSM.Instance.playerAnswered && 
            GSM.Instance.result.emotion_guessed.emotion
            != GSM.Instance.currentEmotion
        )
        {
            buttonText.text = $" Almost! The correct emotion was {GSM.Instance.currentEmotion} ";
        }
    }
    // --------------------------------------------------------------------------------------------
    public void OutroHandler()
    {
        GSM.Instance.readyToMoveOn = true;
    }

}
