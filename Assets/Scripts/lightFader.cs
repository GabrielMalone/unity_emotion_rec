using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class lightFader : MonoBehaviour
{
    private Light2D lampLight;
    public float chanceToFlicker = 0.2f;
    public float flickerTimeMax = 1f;
    private float lampIntensity;
    private float interval = 2f;
    private float intervalStart = 0f;
    private bool runningCoroutine = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lampLight = gameObject.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {   

        if (Time.time - intervalStart > interval && !runningCoroutine)
        {
            intervalStart = Time.time;
            runningCoroutine = true;
            StartCoroutine(LampFlicker());
        }
   
    }

    public IEnumerator LampFlicker()
    {
        if (Random.value < chanceToFlicker)
        {
            lampIntensity = lampLight.intensity;

            float ogIntensity = lampLight.intensity;
    
            float flickerStartTime = Time.time;

            while (Time.time - flickerStartTime < flickerTimeMax)
            {
                float t = (Time.time - flickerStartTime) / flickerTimeMax;
                lampIntensity = Mathf.Lerp(ogIntensity, 0, t);
                lampLight.intensity = lampIntensity;
                yield return null;
            }

            flickerStartTime = Time.time;

            while (Time.time - flickerStartTime < flickerTimeMax)
            {
                float t = (Time.time - flickerStartTime) / flickerTimeMax;
                lampIntensity = Mathf.Lerp(0, ogIntensity, t);
                lampLight.intensity = lampIntensity;
                yield return null;
            }

            lampLight.intensity = ogIntensity;
        }
        runningCoroutine = false;
    }
}
