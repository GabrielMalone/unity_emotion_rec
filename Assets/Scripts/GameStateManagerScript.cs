using UnityEngine;

public static class GameStateManagerScript
{
    //allows these values to be access from any script without needing to be declared
    // you can call any value with just GameStateManagerScript.{valueName}

    [Header("Game State Settings")]
    public static string playerName;
    public static GameObject player;
    public static DialougeHandler dialougeHandler;
    public static ScreenFader screenFader;
    public static GameObject firstStatue;
    public static GameObject fireflyManager;
    private static bool playerReachedFirstStatue = false;
}
