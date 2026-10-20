using System;
using UnityEngine;

public class GameStateManagerScript : MonoBehaviour
{

    [Header("Game State Settings")]
    public string playerName;
    public GameObject player;
    public GameObject firstStatue;
    public GameObject fireflyManager;
    public String [] introAreaEmotionOptions;
    public bool playerControllerDisabled = false;
    private bool playerReachedFirstStatue = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
