using UnityEngine;
using GSM = GameStateManagerScript;
public class LevelExit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;
        Debug.Log("Trying to Exit!");
        if (GSM.Instance.numQuestionsCompleted == GSM.Instance.numStatuesInLvl_1)
        {
            StartCoroutine(GSM.Instance.SetNewScene());
        }
    }
     

}
