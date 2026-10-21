using System;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using System.IO;

public class GameStateManagerScript : MonoBehaviour
{
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
    public class EmotionResult
    {
        public string playerName;
        public float timeCompleted;
        public string emotionName;
        public bool correct;
        public int numOptionsPresent;
        public bool hasVideo;
        public bool hasAudio;
        public bool hasText;
        public int numOptionsGiven;
        public List<string> options;
    }

    [Header("Game State Settings")]
    public List<EmotionResult> results = new List<EmotionResult>();
    public string playerName;
    public GameObject player;
    public GameObject firstStatue;
    public GameObject fireflyManager;
    public String [] introAreaEmotionOptions;
    public String currentEmotion;
    public String [] emotionOptions;
    public VideoClip [] emotionVideos;
    public AudioClip [] emotionAudios;
    public int totalNumQuestions;
    public int numQuestionsCompleted;
    public bool playerControllerDisabled = false;
    public bool playerAnsweredCorrectly = false;
    private bool playerReachedFirstStatue = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialAreaSetUp()
    {
        // 1 - get all the statue game objects
        // 2 - get all the emotion options for init area
        // 3 - determine how often we want each emotion to show up
        // 4 - determine which type of question player is to get (video, audio, text)
        // 5 - get the video / audio / text that corresponds to results from above
        // 6 - apply that media to the quiz canvas stuff
    }


    public void SaveResults()
    {
        // results to JSON with pretty print = true
        string json = JsonUtility.ToJson(
            new ResultsWrapper { results = this.results },
            true
        );

        // unique filename for each participant/session
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"emotionResults_{timestamp}.json";

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
}
