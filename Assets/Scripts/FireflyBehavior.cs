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

    private GameObject player;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Light2D pointLight;
    private Transform fireflyVisual;

    private float twinkleOffset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        agent = GetComponentInChildren<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        fireflyVisual = spriteRenderer.transform;

        pointLight = GetComponentInChildren<Light2D>();

        twinkleOffset = Random.Range(0f, Mathf.PI * 2f);

        SetRandomSize();
    }

    void Update()
    {
        Twinkle();

        // Uncomment when you want them to follow the player
        // if (player != null)
        // {
        //     agent.SetDestination(player.transform.position);
        // }
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
}