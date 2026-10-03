using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDown2DJump : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D bodyCollider;

    [Header("References")]
    public Transform spriteTransform;

    [Header("Jump Settings")]
    public float jumpDuration = 0.6f;
    public float peakHeight = 2.0f;
    public float airborneSpeedMultiplier = 1.3f;
    public int sortingOrder = 10;

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;

    [Header("Collision Layers")]
    public string obstacleLayerName = "Obstacles";

    [Header("Other")]
    private bool isJumping = false;
    private TrailRenderer trail;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        trail = GetComponent<TrailRenderer>();

        spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            !isJumping)
        {
            StartCoroutine(PerformJumpArc());
        }
    }

    private IEnumerator PerformJumpArc()
    {
        isJumping = true;

        trail.emitting = false;

        spriteRenderer.sortingOrder =
            originalSortingOrder + sortingOrder;

        Vector2 travelDirection = rb.linearVelocity.normalized;
        float originalSpeed = rb.linearVelocity.magnitude;

        // Ignore obstacles while jumping.
        int obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);

        if (obstacleLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(
                gameObject.layer,
                obstacleLayer,
                true
            );
        }

        float timer = 0f;

        Vector3 originalLocalPos = spriteTransform.localPosition;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float progress = timer / jumpDuration;

            // Creates the parabolic jump arc.
            float heightOffset =
                1.2f * peakHeight * progress * (1f - progress);

            // Visually move the sprite upward/rightward
            // without moving the Rigidbody2D itself.
            spriteTransform.localPosition = new Vector3(
                originalLocalPos.x + (heightOffset / 2f),
                originalLocalPos.y + heightOffset,
                originalLocalPos.z
            );

            // Maintain movement while airborne.
            if (travelDirection != Vector2.zero)
            {
                rb.linearVelocity =
                    travelDirection *
                    (originalSpeed * airborneSpeedMultiplier);
            }

            yield return null;
        }

        // Put visual back in its normal position.
        spriteTransform.localPosition = originalLocalPos;

        // Turn obstacle collisions back on.
        if (obstacleLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(
                gameObject.layer,
                obstacleLayer,
                false
            );
        }

        trail.emitting = true;

        spriteRenderer.sortingOrder = originalSortingOrder;

        isJumping = false;
    }
}