using UnityEngine;
using TMPro;
using GSM = GameStateManagerScript;

public class StatuesRemainingScript : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GSM.Instance.lvl1)
            textBox.text = $"Statues Remaining: {GSM.Instance.numStatuesInLvl_1 - GSM.Instance.numQuestionsCompleted}";
        if (GSM.Instance.lvl2)
            textBox.text = $"Statues Remaining: {GSM.Instance.numStatuesInLvl_2 - (GSM.Instance.numQuestionsCompleted - GSM.Instance.numStatuesInLvl_1)}";
    }
}
