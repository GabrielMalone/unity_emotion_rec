using System;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using System.IO;

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
    }

    // --------------------------------------------------------------------------------------------
    // create a class that can be converted to JSON
    // JSON utility in c sharp
    // has to work on an object
    // not directly on a list
    // so this is the hack
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
        public bool correct;
        public bool hasVideo;
        public bool hasAudio;
        public bool hasText;
        public string video_title;
        public int questionNumber;
        public List<EmotionGuess> emotionsGuessed = new List<EmotionGuess>();
    }
    // --------------------------------------------------------------------------------------------
    // GAME STATE SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Game State Settings")]
    public List<EmotionResult> results = new List<EmotionResult>();

    public string playerName;
    public GameObject player;
    public GameObject firstStatue;
    public GameObject fireflyManager;
    // --------------------------------------------------------------------------------------------
    // EMOTION SETTINGS
    // --------------------------------------------------------------------------------------------
    [Header("Emotion Settings")]
    public string[] introAreaEmotionOptions;
    public string currentEmotion;
    public string[] emotionOptions;
    public VideoClip[] happyVideos;
    public VideoClip[] sadVideos;
    public VideoClip[] angryVideos;
    public VideoClip[] afraidVideos;
    public AudioClip[] emotionAudios;
    public VideoPlayer videoPlayer;
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

    [Header("Telemetry Per Quiz")]
    public EmotionResult result = new EmotionResult();
    // --------------------------------------------------------------------------------------------
    void Start()
    {
        PickNextEmotion();
        SetEmotionVideo();
    }

    void Update()
    {

    }
    // --------------------------------------------------------------------------------------------

    public void InitialAreaSetUp()
    {
        // 1 - get all the statue game objects
        // 2 - get all the emotion options for init area
        // 3 - determine how often we want each emotion to show up
        // 4 - determine which type of question player is to get (video, audio, text)
        // 5 - get the video / audio / text that corresponds to results from above
        // 6 - apply that media to the quiz canvas stuff
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
        // for now just pick random
        currentEmotion = emotionOptions[UnityEngine.Random.Range(0, emotionOptions.Length)];
        Debug.Log($"Current emotion picked: {currentEmotion}");
    }
    // --------------------------------------------------------------------------------------------
    public void SetEmotionVideo()
    {
        int idx = UnityEngine.Random.Range(0,happyVideos.Length); 
        switch(currentEmotion)
        {
            case "HAPPY" :  videoPlayer.clip = happyVideos[idx];    return;
            case "SAD"   :  videoPlayer.clip = sadVideos[idx];      return;
            case "ANGRY" :  videoPlayer.clip = angryVideos[idx];    return;
            case "AFRAID":  videoPlayer.clip = afraidVideos[idx];   return;
            default : return;
        }
    }
}