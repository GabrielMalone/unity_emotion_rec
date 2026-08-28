using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;
    public void SwitchScene()// when the a button is pressed
    {
        SceneManager.LoadScene(sceneName); //load the scene named sceneName
    }
}
