using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GSM = GameStateManagerScript;

public class PlayerStartScript : MonoBehaviour
{
    [Header("Player Setup")]
    public TMP_InputField playerNameInput;
    public Button submitPlayerNameButton;
    private bool playerNameSet = false;

    // Start is called once before 
    // the first execution of Update
    //  after the MonoBehaviour is created
    void Start()
    {
        playerNameInput.text = "Enter particpant name";
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNameSet)
        {
            gameObject.SetActive(false);
            GSM.Instance.playerName = playerNameInput.text;
        }
    }


    public void SubmitName()
    {
        Debug.Log("Setting Player Name!");
        playerNameSet = true;
    }

}
