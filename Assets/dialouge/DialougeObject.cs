using UnityEngine;

[CreateAssetMenu(fileName = "Dialouge", menuName = "ScriptableObjects/dialouge")]
public class DialougeObject : ScriptableObject
{
    [System.Serializable]
    public struct Speech
    {
        public Sprite expression;

        [TextArea(5, 20)]
        public string text;

        public bool isQuestion;

        public string[] Answers;

        [TextArea(5, 20)]
        public string wrongText;
    }

    public Speech[] Dialouge; 
}
