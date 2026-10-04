using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class FireflyBehavior : MonoBehaviour
{
    [Header("Firefly Behavior")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    [Header("Twinkle Settings")]
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;
    public float minLightIntensity = 0.2f;
    public float maxLightIntensity = 1f;
    public float twinkleSpeed = 2f;

    [Header("Speed-Audio Settings")]
    public float minSpeed = 1f;
    public float maxSpeed = 20f;
    public float minVolume = 0.05f;
    public float maxVolume = 0.10f;
    private GameObject AudioManager;
    private AudioAnalyzer audioAnalyzer;
    private float prevFrequency;
    private float targetSpeed;

    public float frequencySampleInterval = 0.75f;
    public float frequencyChangeForMaxSpeed = 500f;
    public float minFireflySpeed = 1f;
    public float maxFireflySpeed = 20f;

    private float frequencySampleTimer;

    private GameObject[] fireflyAttractors;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Light2D pointLight;
    private Transform fireflyVisual;

    private float twinkleOffset;

    void Start()
    {
        fireflyAttractors = GameObject.FindGameObjectsWithTag("FireflyAttractor");

        agent = GetComponentInChildren<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        fireflyVisual = spriteRenderer.transform;

        pointLight = GetComponentInChildren<Light2D>();

        twinkleOffset = Random.Range(0f, Mathf.PI * 2f);

        AudioManager = GameObject.Find("AudioManager");
        audioAnalyzer = AudioManager.GetComponent<AudioAnalyzer>();
        prevFrequency = audioAnalyzer.GetDominantFrequency();
        SetRandomSize();
    }

    void Update()
    {
        Twinkle();
        FollowNearestAttractor();
        AudioFX();
    }

    void LateUpdate()
    {
        pointLight.transform.position = fireflyVisual.position;
    }

    void SetRandomSize()
    {
        float randomScale = Random.Range(minScale, maxScale);

        transform.localScale = new Vector3(
            randomScale,
            randomScale,
            1f
        );
        pointLight.transform.localScale = new Vector3(
            randomScale,
            randomScale,
            1f
        );
    }

    void Twinkle()
    {
        float t = (Mathf.Sin(Time.time * twinkleSpeed + twinkleOffset) + 1f) / 2f;

        // Light
        pointLight.intensity = Mathf.Lerp(
            minLightIntensity,
            maxLightIntensity,
            t
        );

        // Sprite
        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        spriteRenderer.color = color;
    }


    void FollowNearestAttractor()
    {
        if (fireflyAttractors != null)
        {
            GameObject closestAttractor = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject fireflyAttractor in fireflyAttractors)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    fireflyAttractor.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAttractor = fireflyAttractor;
                }
            }

            if (closestAttractor != null)
            {
                agent.SetDestination(closestAttractor.transform.position);
            }
        }
    }

    void AudioFX()
    {
        if (audioAnalyzer == null)
            return;

        frequencySampleTimer += Time.deltaTime;

        // Every 0.75 seconds, calculate a new target speed
        if (frequencySampleTimer >= frequencySampleInterval)
        {
            float volumeAmount = Mathf.InverseLerp(
                minVolume,
                maxVolume,
                audioAnalyzer.volume
            );
            targetSpeed = Mathf.Lerp(
                minSpeed,
                maxSpeed,
                volumeAmount
            );

            frequencySampleTimer = 0f;
        }

        // This part runs EVERY FRAME
        agent.speed = Mathf.Lerp(
            agent.speed,
            targetSpeed,
            Time.deltaTime * 2f
        );
    }

}