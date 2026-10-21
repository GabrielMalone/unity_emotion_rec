using UnityEngine;

public class GameStateInitizlier : MonoBehaviour
{
    public string playerName;
    public GameObject player;
    public DialougeHandler dialougeHandler;
    public ScreenFader screenFader;
    public GameObject firstStatue;
    public GameObject fireflyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameStateManagerScript.playerName = playerName;
        GameStateManagerScript.player = player;
        GameStateManagerScript.dialougeHandler = dialougeHandler;
        GameStateManagerScript.screenFader = screenFader;
        GameStateManagerScript.firstStatue = firstStatue;
        GameStateManagerScript.fireflyManager = fireflyManager;
    }
}
