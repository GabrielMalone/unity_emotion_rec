using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for the Image component
using UnityEngine.Video;

using GSM = GameStateManagerScript;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image fullScreenFadeImage;
    [SerializeField] private float fadeDuration = 3f;
    public GameObject player;
    // bg music source
    public AudioSource audioSource;
    public AudioSource emotionAudioSource;
    public GameObject GameStateManager;
    // these will have to be dynamically controlled by 
    // the gamestate manager
    // maybe turn these into a dictionary of gameobjects
    // like you did w/ the alphabet in other game
    public VideoPlayer videoTest;
    public RawImage videoImage;
    public Image stillImage;
    public RawImage inGameVideoImage;
    public Image inGameStillImage;
    public Transform mediaPosition;
    public GameObject DialogueHolder;
    public GameObject introTextBox;
    public GameObject introTextBoxCornerImage;
    public GameObject introSureButton;
    public GameObject Qiuz;
    public GameObject statueFace;

    private PlayerController playerController;
    public bool readyForQuiz = false;
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        // changing from full screen to in game
        videoImage = inGameVideoImage;
        stillImage = inGameStillImage;
        // make sure everything is visible for video playback
        videoTest.renderMode = VideoRenderMode.RenderTexture;
        // cast to rendertexture
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
    // --------------------------------------------------------------------------------------------
    public void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished playing!");
        videoImage.gameObject.SetActive(false);
        FadeFromBlack();
        readyForQuiz = true;
    }
    // --------------------------------------------------------------------------------------------
    public void OnImageFinished()
    {
        Debug.Log("Image finished displaying");
        stillImage.gameObject.SetActive(false);
        readyForQuiz = true;
    }
    // --------------------------------------------------------------------------------------------
    public void OnAudioFinished()
    {
        Debug.Log("Audio finished displaying");
        readyForQuiz = true;
        FadeFromBlack();
    }
    // --------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        videoTest.loopPointReached -= OnVideoFinished;
    }
    // --------------------------------------------------------------------------------------------
    public void FadeToBlack()
    {
        StartCoroutine(FadeRoutine(0f, 1f));
    }
    // --------------------------------------------------------------------------------------------
    public void FadeFromBlack()
    {
        StartCoroutine(FadeRoutine(1f, 0f));
    }
    // --------------------------------------------------------------------------------------------
    public IEnumerator SimpleFade(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fullScreenFadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsedTime / fadeDuration
            );
            fullScreenFadeImage.color = color;
            // fade game volume too
            if (targetAlpha == 1f)
            {
                audioSource.volume = Mathf.Lerp(
                    1,
                    0,
                    elapsedTime / fadeDuration
                );
            }
            else
            {
                audioSource.volume = Mathf.Lerp(
                    0,
                    1,
                    elapsedTime / fadeDuration
                );
            }
            yield return null;
        }

        // snap final values into place
        color.a = targetAlpha;
        fullScreenFadeImage.color = color;
    }
    // --------------------------------------------------------------------------------------------
    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {

        Qiuz.SetActive(false);

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        // replay selected
        if (targetAlpha == 0f)
        {
            Qiuz.SetActive(true);
        }

        // remove image media from UI if used
        if (targetAlpha == 0f && stillImage.gameObject.activeSelf)
        {
            OnImageFinished();
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsedTime / fadeDuration
            );
            fadeImage.color = color;
            // fade game volume too
            if (targetAlpha == 1f)
            {
                audioSource.volume = Mathf.Lerp(
                    1,
                    0,
                    elapsedTime / fadeDuration
                );
            }
            else
            {
                audioSource.volume = Mathf.Lerp(
                    0,
                    1,
                    elapsedTime / fadeDuration
                );
            }
            yield return null;
        }

        // snap final values into place
        color.a = targetAlpha;
        fadeImage.color = color;

        if (targetAlpha == 1f)
        {
            // if faded to black then play 
            // media component
            PlayMediaVariation();
        }

    }
    // --------------------------------------------------------------------------------------------
    public void PlayMediaVariation()
    {
        Debug.Log($"next media variation: {GSM.Instance.nextMediaVariation}");
        // video
        if (GSM.Instance.nextMediaVariation == 0)
        {
            GSM.Instance.ClearPreviousVideoTexture();
            stillImage.gameObject.SetActive(false);
            videoImage.gameObject.SetActive(true);
            videoImage.transform.SetAsLastSibling();
            videoTest.Play();
        }
        // still image
        if (GSM.Instance.nextMediaVariation == 1)
        {
            statueFace.SetActive(false);
            videoImage.gameObject.SetActive(false);
            stillImage.gameObject.SetActive(true);
            stillImage.transform.SetAsLastSibling();
            StartCoroutine(WaitForImageFinish());

        }
        // audio
        if (GSM.Instance.nextMediaVariation == 2)
        {
            emotionAudioSource.Play();
            StartCoroutine(WaitForAudioFinished());
        }

        // ----------------------------------------------------------------------------------------
        IEnumerator WaitForImageFinish()
        {
            yield return new WaitForSeconds(GSM.Instance.timeImagesDisplayed);
            StartCoroutine(FadeRoutine(1, 0));
        }
        // ----------------------------------------------------------------------------------------
        IEnumerator WaitForAudioFinished()
        {
            yield return new WaitUntil(() => !emotionAudioSource.isPlaying);
            OnAudioFinished();
        }
    }
    // --------------------------------------------------------------------------------------------
}
// afraid video, 