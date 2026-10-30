using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.AI;

public class GameStateManagerScript : MonoBehaviour
{
    // --------------------------------------------------------------------------------------------
    // SINGLETON (single instance of class / global access)
    // --------------------------------------------------------------------------------------------
    public static GameStateManagerScript Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // persist from scene to scene
        DontDestroyOnLoad(gameObject);
    }
    // --------------------------------------------------------------------------------------------
    // TELEMETRY STRUCTURES
    // --------------------------------------------------------------------------------------------
    // create a class that can be converted to JSON
    // JSON utility in c sharp
    // has to work on an object
    // not directly on a list
    // so this is the hack
    // --------------------------------------------------------------------------------------------
    [System.Serializable]
    public class ResultsWrapper
    {
        public List<EmotionResult> results;
        public List<KeyStroke> keyStrokes;
    }

    [System.Serializable]
    public class KeyStroke
    {
        public string key;
        public float time;
    }

    [System.Serializable]
    public class EmotionGuess
    {
        public string emotion;
        public float time;
    }

    [System.Serializable]
    public class EmotionResult
    {
        public string playerName;
        public float timeGameStarted;
        public float timeQuizStarted;
        public float timeQuizCompleted;
        public string emotionName;
        public bool isVideo = false;
        public bool isAudio = false;
        public bool isStillImage = false;
        public string video_title;
        public string audio_title;
        public string still_image_title;
        public int questionNumber;
        public bool replayUsed = false;
        public EmotionGuess emotion_guessed;
        public bool wasCorrect;
    }
    // --------------------------------------------------------------------------------------------
    // GAME STATE SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Game State Settings")]
    public List<EmotionResult> results = new List<EmotionResult>();
    public List<KeyStroke> keyStrokes = new List<KeyStroke>();

    public GameObject lastStatueInteractedWith;

    public int happyVideoIndex;
    public int sadVideoIndex;
    public int angryVideoIndex;
    public int afraidVideoIndex;

    public int happyAudioIndex;
    public int sadAudioIndex;
    public int angryAudioIndex;
    public int afraidAudioIndex;

    public int happyImageIndex;
    public int sadImageIndex;
    public int angryImageIndex;
    public int afraidImageIndex;

    public string playerName;
    public GameObject player;
    public GameObject firstStatue;
    public GameObject fireflyManager;
    // --------------------------------------------------------------------------------------------
    // LEVEL SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Level Settings")]
    public bool lvl1 = false;
    public bool lvl2 = false;
    public bool lvl3 = false;

    public int numStatuesInLvl_1 = 4;
    public int numStatuesInLvl_2 = 20;
    public int numStatuesInLvl_3 = 12;

    public string[] emotionsTestedInLvl_1;
    public string[] emotionsTestedInLvl_2;
    public string[] emotionsTestedInLvl_3;

    // --------------------------------------------------------------------------------------------
    // EMOTION SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Emotion Settings")]
    public string currentEmotion;
    public string[] emotionOptions;

    public VideoClip[] happyVideos;
    public VideoClip[] sadVideos;
    public VideoClip[] angryVideos;
    public VideoClip[] afraidVideos;

    public AudioClip[] happyAudios;
    public AudioClip[] sadAudios;
    public AudioClip[] angryAudios;
    public AudioClip[] afraidAudios;

    public Sprite[] happyImages;
    public Sprite[] sadImages;
    public Sprite[] afraidImages;
    public Sprite[] angryImages;

    public VideoPlayer videoPlayer;
    public Image still_image_holder;
    public AudioSource emotionAudioPlayer;

    // --------------------------------------------------------------------------------------------
    // QUESTION SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Question Settings")]
    public int totalNumQuestions;
    public int numQuestionsCompleted;
    public bool agreedToHelp = false;
    public bool playerControllerDisabled = false;
    public bool playerAnswered = false;
    public float timeGameStarted;
    public float timeImagesDisplayed = 3f;
    public bool replaySelected = false;
    public bool readyToMoveOn = false;

    [Header("Randomization of Emotions Settings")]
    public List<string> seenEmotions = new List<string>();
    public int happySeen = 0;
    public int sadSeen = 0;
    public int angrySeen = 0;
    public int afraidSeen = 0;
    public int lvl1_happyEmotionsLimit;
    public int lvl1_afraidEmotionsLimit;
    public int lvl2_happyEmotionsLimit;
    public int lvl2_afraidEmotionsLimit;
    public int lvl2_angryEmotionsLimit;
    public int lvl2_sadEmotionsLimit;
    public int nextMediaVariation = 0;


    [Header("Particle FX")]
    public ParticleSystem ConfettiA;
    public ParticleSystem ConfettiB;
    public ParticleSystem Smoke;

    [Header("Lvl Settings")]
    public GameObject lvl1Light;
    public GameObject lvl1GlobalLight;
    public GameObject fog1;
    public GameObject fog2;
    public ParticleSystem dust;
    public GameObject lvl2Environment;
    public Transform lvl2spawnPoint;
    public GameObject startArea;

    // --------------------------------------------------------------------------------------------
    [Header("Telemetry Per Quiz")]
    public EmotionResult result = new EmotionResult();
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        SetNextRandomEmotion();
        SetMedia();
    }
    // --------------------------------------------------------------------------------------------
    void Update()
    {

    }
    // --------------------------------------------------------------------------------------------
    public void SaveResults()
    {
        // results to JSON with pretty print = true
        string json = JsonUtility.ToJson(
            new ResultsWrapper { results = this.results, keyStrokes = this.keyStrokes },
            true
        );

        // unique filename for each participant/session
        // string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{playerName}_emotionResults.json";

        // save the file
        // C:\Users\<Username>\AppData\LocalLow\<CompanyName>\<ProductName>\emotionResults.json
        // ~/Library/Application Support/<CompanyName>/<ProductName>/emotionResults.json
        string filePath = Path.Combine(
            Application.persistentDataPath,
            fileName
        );

        File.WriteAllText(filePath, json);
        Debug.Log("Results saved to: " + filePath);
    }
    // --------------------------------------------------------------------------------------------
    // Randomization of emotions tracking structures
    // --------------------------------------------------------------------------------------------
    public void SetNextRandomEmotion()
    {

        if (lvl1 && numQuestionsCompleted == numStatuesInLvl_1) return;
        if (lvl2 && numQuestionsCompleted == numStatuesInLvl_2) return;

        int attempts = 0;
        while (attempts ++ < 10_000)
        {
            // pick random emotion for this level
            string rnd_emotion = PickRandomEmotion();
            Debug.Log($"attempting {rnd_emotion}");
            if (lvl1 && rnd_emotion == "HAPPY" && happySeen >= lvl1_happyEmotionsLimit)
            {
                // if seen too many times go back and pick another random emotion
                Debug.Log($"too many {rnd_emotion}");
                continue;
            }
            if (lvl1 && rnd_emotion == "AFRAID" && afraidSeen >= lvl1_afraidEmotionsLimit)
            {
                // if seen too many times go back and pick another random emotion
                Debug.Log($"too many {rnd_emotion}");
                continue;
            }
            if (lvl2 && rnd_emotion == "HAPPY" && happySeen >= lvl2_happyEmotionsLimit)
            {
                // if seen too many times go back and pick another random emotion
                Debug.Log($"too many {rnd_emotion}");
                continue;
            }
            if (lvl2 && rnd_emotion == "AFRAID" && afraidSeen >= lvl2_afraidEmotionsLimit)
            {
                // if seen too many times go back and pick another random emotion
                Debug.Log($"too many {rnd_emotion}");
                continue;
            }
            if (lvl2 && rnd_emotion == "SAD" && sadSeen >= lvl2_sadEmotionsLimit)
            {
                // if seen too many times go back and pick another random emotion
                Debug.Log($"too many {rnd_emotion}");
                continue;
            }
            if (lvl2 && rnd_emotion == "ANGRY" && angrySeen >= lvl2_angryEmotionsLimit)
            {
                // if seen too many times go back and pick another random emotion
                Debug.Log($"too many {rnd_emotion}");
                continue;
            }
            // this not necessary in level one, but if it changes in the future
            // can keep this here
            if (!TooManyConsecutiveEmotions(rnd_emotion))
            {
                currentEmotion = rnd_emotion;
                Debug.Log($"Setting new rnd emotion: {currentEmotion}");
                seenEmotions.Add(currentEmotion);
                // add emotion to appropriate seen list
                switch (currentEmotion)
                {
                    case "HAPPY":
                        happySeen++;
                        return;
                    case "SAD":
                        sadSeen++;
                        return;
                    case "ANGRY":
                        angrySeen++;
                        return;
                    case "AFRAID":
                        afraidSeen++;
                        return;
                    default:
                        return;
                }
            }
        }
        // will need to make sure media doesn't go out of bounds for this one though
        currentEmotion = PickRandomEmotion();
        Debug.Log($"rnd emotion with constraints faled picking {currentEmotion}");
        
    }
    // --------------------------------------------------------------------------------------------
    public bool TooManyConsecutiveEmotions(string rnd_emotion)
    {

        if (seenEmotions.Count < 2) return false;

        string prev2 = seenEmotions[^2];
        string prev1 = seenEmotions[^1];

        Debug.Log($"last two emotions: {prev2} | {prev1}");

        switch (rnd_emotion)
        {
            case "HAPPY":
                if (happySeen < 2)
                    return false;
                return prev2 == prev1 && prev1 == rnd_emotion;
            case "SAD":
                if (sadSeen < 2)
                    return false;
                return prev2 == prev1 && prev1 == rnd_emotion;
            case "ANGRY":
                if (angrySeen < 2)
                    return false;
                return prev2 == prev1 && prev1 == rnd_emotion;
            case "AFRAID":
                if (afraidSeen < 2)
                    return false;
                return prev2 == prev1 && prev1 == rnd_emotion;

            default:
                Debug.Log("something went wrong in TooManyConsecutiveEmotions");
                return true;
        }
    }
    // --------------------------------------------------------------------------------------------
    public string PickRandomEmotion()
    {
        if (lvl1)
            return emotionsTestedInLvl_1[Random.Range(0, emotionsTestedInLvl_1.Length)];
        if (lvl2)
            return emotionsTestedInLvl_2[Random.Range(0, emotionsTestedInLvl_2.Length)];
        return "";
    }
    // --------------------------------------------------------------------------------------------
    public void SetMedia()
    {
        switch (currentEmotion)
        {
            case "HAPPY":
                if (happySeen % 3 == 1)
                {
                    videoPlayer.clip = happyVideos[happyVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    nextMediaVariation = 0;
                }
                else if (happySeen % 3 == 2)
                {
                    still_image_holder.sprite = happyImages[happyImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    nextMediaVariation = 1;
                }
                else if (happySeen % 3 == 0)
                {
                    emotionAudioPlayer.clip = happyAudios[happyAudioIndex++];
                    result.audio_title = emotionAudioPlayer.clip.name;
                    nextMediaVariation = 2;
                }
                return;
            case "SAD":
                if (sadSeen % 3 == 1)
                {
                    videoPlayer.clip = sadVideos[sadVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    nextMediaVariation = 0;
                }
                else if (sadSeen % 3 == 2)
                {
                    still_image_holder.sprite = sadImages[sadImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    nextMediaVariation = 1;
                }
                else if (sadSeen % 3 == 0)
                {
                    emotionAudioPlayer.clip = sadAudios[sadAudioIndex++];
                    result.audio_title = emotionAudioPlayer.clip.name;
                    nextMediaVariation = 2;
                }
                return;
            case "ANGRY":
                if (angrySeen % 3 == 1)
                {
                    videoPlayer.clip = angryVideos[angryVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    nextMediaVariation = 0;
                }
                else if (angrySeen % 3 == 2)
                {
                    still_image_holder.sprite = angryImages[angryImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    nextMediaVariation = 1;
                }
                else if (angrySeen % 3 == 0)
                {
                    emotionAudioPlayer.clip = angryAudios[angryAudioIndex++];
                    result.audio_title = emotionAudioPlayer.clip.name;
                    nextMediaVariation = 2;
                }
                return;
            case "AFRAID":
                if (afraidSeen % 3 == 1)
                {
                    videoPlayer.clip = afraidVideos[afraidVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    nextMediaVariation = 0;
                }
                else if (afraidSeen % 3 == 2)
                {
                    still_image_holder.sprite = afraidImages[afraidImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    nextMediaVariation = 1;
                }
                else if (afraidSeen % 3 == 0)
                {
                    emotionAudioPlayer.clip = afraidAudios[afraidAudioIndex++];
                    result.audio_title = emotionAudioPlayer.clip.name;
                    nextMediaVariation = 2;
                }
                return;
            default: return;
        }
    }
    // --------------------------------------------------------------------------------------------
    public void ClearPreviousVideoTexture()
    {
        videoPlayer.Stop();
        RenderTexture rt = videoPlayer.targetTexture;
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = previous;
    }
    // --------------------------------------------------------------------------------------------
    public IEnumerator SetNewScene()
    {
        ScreenFader sf = gameObject.GetComponent<ScreenFader>();
        StartCoroutine(sf.SimpleFade(0, 1));
        playerControllerDisabled = true;
        yield return new WaitForSeconds(1.5f);

        lvl1GlobalLight.SetActive(false);
        lvl1Light.SetActive(false);
        fog1.SetActive(false);
        fog2.SetActive(false);
        dust.Stop();
        lvl1 = false;
        lvl2 = true;
        lvl2Environment.SetActive(true);

        // move the firelfly attractors and the fireflies

        GameObject[] fireflyattractors = GameObject.FindGameObjectsWithTag("FireflyAttractor");
        GameObject[] fireflies = GameObject.FindGameObjectsWithTag("Firefly");

        foreach (GameObject fireFly in fireflies)
        {
            Destroy(fireFly);
        }

        foreach (GameObject attractor in fireflyattractors)
        {
            NavMeshAgent agent = attractor.GetComponent<NavMeshAgent>();
            // cant use transform position change on agents I guess
            agent.Warp(lvl2spawnPoint.position);
            attractor.GetComponent<FireflyAttractorBehavior>().environmentArea =
                lvl2spawnPoint.GetComponent<CircleCollider2D>();
        }

        player.transform.position = lvl2spawnPoint.position;
        // set sprite layer to above bridges in lvl 2
        player.GetComponent<SpriteRenderer>().sortingOrder = 4;
        player.GetComponent<Rigidbody2D>().linearDamping = 10;
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(sf.SimpleFade(1, 0));
        playerControllerDisabled = false;
    }
    // --------------------------------------------------------------------------------------------
}