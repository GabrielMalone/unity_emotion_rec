using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for the Image component
using UnityEngine.Video;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    public GameObject player;
    public AudioSource audioSource;
    public VideoPlayer videoTest;
    public RawImage videoImage;

    private PlayerController playerController;

    void Start()
    {   
        // make sure everything is visible for video playback
        videoTest.renderMode = VideoRenderMode.RenderTexture;
        videoTest.targetTexture = videoImage.texture as RenderTexture;
        videoImage.gameObject.SetActive(false);

        // so we can disable controls while video plays
        playerController = player.GetComponent<PlayerController>();

        //  image starts  transparent
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        // add method to end event
        videoTest.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished playing!");
        videoImage.gameObject.SetActive(false);
        FadeFromBlack();
        // will still need to lock movement controlls at this point until questions completed
    }

    void OnDestroy()
    {
        videoTest.loopPointReached -= OnVideoFinished;
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

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;

            if (targetAlpha == 1f)
            {
                audioSource.volume = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            } 
            else
            {
                audioSource.volume = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            }
            yield return null;
        }
   
        color.a = targetAlpha;
        fadeImage.color = color;
        if (targetAlpha == 1f)
        {
            playerController.controllerDisabled = true;
            // if faded to black then play movie
            videoImage.gameObject.SetActive(true);
            videoImage.transform.SetAsLastSibling();
            videoTest.Play();
        } 
        else
        {
            playerController.controllerDisabled = false;
        }
    }
}
