using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class GameStateManagerScript : MonoBehaviour
{
    //--------------------------------------------------------------------------------------------

    // TO DO for Wednesday --> 12-4:30
    // ____________________________________________________________
    // --> tutorial area
    // --> don't randomize the emotions they see
    // --> set up the still images form the NIH images
    // --> get elliot to set up the audio
    // --> set up ability to replay audio and video
    // --> make it obvious that the exit is needed to go through
    // --> how many statues remain 
    // --> how long faces should be up 3-5 sec ???
    // --> loop music!
    // --> key log every button 
    //  ____________________________________________________________
    // --> play video 
    // --> 5 options (1 being replay) 
    // --> play again if clicked
    // --> 4 options 
    // --> but only one guess
    // --> smoke and pop / create confetti 
    // --> departing message for right and wrong (with explicitly stating correct answer)
    // ____________________________________________________________
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
        public float timeStarted;
        public float timeCompleted;
        public string emotionName;
        public bool isVideo = false;
        public bool isAudio = false;
        public bool isStillImage = false;
        public string video_title;
        public string audio_title;
        public string still_image_title;
        public int questionNumber;
        public bool replayUsed = false;
        public List<EmotionGuess> emotionsGuessed = new List<EmotionGuess>();
    }
    // --------------------------------------------------------------------------------------------
    // GAME STATE SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Game State Settings")]
    public List<EmotionResult> results = new List<EmotionResult>();

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
    public int numStatuesInLvl_2 = 8;
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

    public Sprite [] happyImages;
    public Sprite[] sadImages;
    public Sprite[] afraidImages;
    public Sprite[] angryImages;

    public VideoPlayer videoPlayer;
    public Image still_image_holder;
    public AudioSource emotionAudioPlayer;

    public VideoClip lastVideoPlayed;
    public AudioClip lastAudioPlayed;
    public Image lastImagePlayed;
    // --------------------------------------------------------------------------------------------
    // QUESTION SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Question Settings")]
    public int totalNumQuestions;
    public int numQuestionsCompleted;
    public bool agreedToHelp = false;
    public bool playerControllerDisabled = false;
    public bool playerAnsweredCorrectly = false;
    public float timeGameStarted;
    public float timeImagesDisplayed = 3f;
    public bool replaySelected = false;
    // --------------------------------------------------------------------------------------------
    public string[] incorrectResponses = {
            "Not quite! Try again!",
            "That's not it! Give it another try!",
            "Not this time! Keep trying!",
            "That's incorrect, but you've got this!",
            "Not quite right! Take another look!",
            "Oops, that's not the one!",
            "Good try, but that's not correct!",
            "Not quite! What else could it be?",
            "That's not the right emotion! Try again!",
            "Not this one! Have another guess!",
            "That's not quite right! Keep going!",
            "Nice effort, but not quite!",
            "Not the right answer! Try again!",
            "That's not it! Take your time!",
            "Incorrect, but don't give up!"
        };
    // --------------------------------------------------------------------------------------------
    [Header("Telemetry Per Quiz")]
    public EmotionResult result = new EmotionResult();
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        PickNextEmotion();
        SetEmotionMedia();
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
            new ResultsWrapper { results = this.results },
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
    public void PickNextEmotion()
    {
        if (lvl1) currentEmotion = emotionsTestedInLvl_1[UnityEngine.Random.Range(0, emotionsTestedInLvl_1.Length)];
        if (lvl2) currentEmotion = emotionsTestedInLvl_2[UnityEngine.Random.Range(0, emotionsTestedInLvl_2.Length)];
        if (lvl3) currentEmotion = emotionsTestedInLvl_3[UnityEngine.Random.Range(0, emotionsTestedInLvl_3.Length)];
        Debug.Log($"Current emotion picked: {currentEmotion}");
    }
    // --------------------------------------------------------------------------------------------
    public void SetEmotionMedia()
    {
        // video audio text
        if (numQuestionsCompleted % 3 == 0)
        {
            ClearPreviousVideoTexture();
            result.isVideo = true;
            switch (currentEmotion)
            {
                case "HAPPY": 
                    videoPlayer.clip = happyVideos[happyVideoIndex++]; 
                    lastVideoPlayed = videoPlayer.clip;
                    result.video_title = videoPlayer.clip.name;
                    return;
                case "SAD": 
                    videoPlayer.clip = sadVideos[sadVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    lastVideoPlayed = videoPlayer.clip;
                    return;
                case "ANGRY": 
                    videoPlayer.clip = angryVideos[angryVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    lastVideoPlayed = videoPlayer.clip;
                    return;
                case "AFRAID": 
                    videoPlayer.clip = afraidVideos[afraidVideoIndex++];
                    result.video_title = videoPlayer.clip.name;
                    lastVideoPlayed = videoPlayer.clip;
                    return;
                default: return;
            }
        }
        // audio only
        if (numQuestionsCompleted % 3 == 1)
        {
            result.isAudio = true;
            switch (currentEmotion)
            {
                case "HAPPY":
                    emotionAudioPlayer.clip = happyAudios[happyAudioIndex++];
                    result.audio_title = videoPlayer.clip.name;
                    lastAudioPlayed = emotionAudioPlayer.clip;
                    return;
                case "SAD":
                    emotionAudioPlayer.clip = sadAudios[sadAudioIndex++];
                    result.audio_title = videoPlayer.clip.name;
                    lastAudioPlayed = emotionAudioPlayer.clip;
                    return;
                case "ANGRY":
                    emotionAudioPlayer.clip = angryAudios[angryAudioIndex++];
                    result.audio_title = videoPlayer.clip.name;
                    lastAudioPlayed = emotionAudioPlayer.clip;
                    return;
                case "AFRAID":
                    emotionAudioPlayer.clip = afraidAudios[afraidAudioIndex++];
                    result.audio_title = videoPlayer.clip.name;
                    lastAudioPlayed = emotionAudioPlayer.clip;
                    return;
                default: return;
            }
        }
        // image only
        if (numQuestionsCompleted % 3 == 2)
        {

            result.isStillImage = true;
            switch (currentEmotion)
            {
                case "HAPPY":
                    still_image_holder.sprite = happyImages[happyImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    lastImagePlayed.sprite = still_image_holder.sprite;
                    return;
                case "SAD":
                    still_image_holder.sprite = sadImages[sadImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    lastImagePlayed.sprite = still_image_holder.sprite;
                    return;
                case "ANGRY":
                    still_image_holder.sprite = angryImages[angryImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    lastImagePlayed.sprite = still_image_holder.sprite;
                    return;
                case "AFRAID":
                    still_image_holder.sprite = afraidImages[afraidImageIndex++];
                    result.still_image_title = still_image_holder.sprite.name;
                    lastImagePlayed.sprite = still_image_holder.sprite;
                    return;
                default: return;
            }
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
} 