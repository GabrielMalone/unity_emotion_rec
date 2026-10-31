using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using GSM = GameStateManagerScript;

public class EndGameScript : MonoBehaviour
{
    // --------------------------------------------------------------------------------------------
    public TextMeshProUGUI exitTextBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        exitTextBox.text = $"Thank you for your help, {GSM.Instance.playerName}!";
    }

    // Update is called once per frame
    // --------------------------------------------------------------------------------------------
    void Update()
    {
        
    }
    // --------------------------------------------------------------------------------------------
    public void RestartGame()
    {
        Debug.Log("restarting game!");
        SceneManager.LoadScene(0);
    }

}
