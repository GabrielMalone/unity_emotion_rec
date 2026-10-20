using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for the Image component
using UnityEngine.InputSystem;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    public GameObject player;
    public AudioSource audioSource;

    private PlayerController playerController;

    void Start()
    {

        playerController = player.GetComponent<PlayerController>();
        //  image starts  transparent
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeRoutine(0f, 1f));
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        float curVolume = audioSource.volume;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;

            if (targetAlpha == 1f)
            {
                audioSource.volume = Mathf.Lerp(curVolume, 0, elapsedTime / fadeDuration);
            }


            yield return null;
        }
        // Snap precisely to the target alpha at the end
        color.a = targetAlpha;
        fadeImage.color = color;
        if (targetAlpha == 1f)
        {
            playerController.controllerDisabled = true;

        } 
        else
        {
            playerController.controllerDisabled = false;
        }
    }
}
