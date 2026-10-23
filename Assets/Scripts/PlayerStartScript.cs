using System.Collections;
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
    [Header("Audio Setup")]
    public AudioSource audioSource;

    // Start is called once before 
    // the first execution of Update
    //  after the MonoBehaviour is created
    void Start()
    {
        playerNameInput.text = "Enter particpant name";
        GSM.Instance.playerControllerDisabled = true;
        GSM.Instance.lvl1 = true;
        audioSource.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNameSet)
        {
            playerNameSet = false;
            StartCoroutine(FadeOut());
        }
    }


    public void SubmitName()
    {
        Debug.Log("Setting Player Name!");
        GSM.Instance.playerName = playerNameInput.text;
        playerNameSet = true;
        playerNameInput.interactable = false;
    }


    IEnumerator FadeOut()
    {
        Image bg = gameObject.GetComponent<Image>();
        Image buttonImage = submitPlayerNameButton.GetComponent<Image>();
        Image inputImage = playerNameInput.GetComponent<Image>();

        Color bgColor = bg.color;
        Color buttonImageColor = buttonImage.color;
        Color inputImageColor = inputImage.color;

        float alpha = bgColor.a;

        float fadeTime = 1f;
        float startTime = Time.time;

        while (Time.time - startTime < fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            bgColor.a = Mathf.Lerp(alpha,0,t);
            buttonImageColor.a = Mathf.Lerp(alpha, 0, t);
            inputImageColor.a = Mathf.Lerp(alpha, 0, t);
            bg.color = bgColor;
            buttonImage.color = buttonImageColor;
            inputImage.color = buttonImageColor;

            audioSource.volume = Mathf.Lerp(
                0,
                1,
                (Time.time-startTime) / fadeTime
            );
            yield return null;
        }
        GSM.Instance.playerControllerDisabled = false;
        gameObject.SetActive(false);
        GSM.Instance.timeGameStarted = Time.time;
    }
}