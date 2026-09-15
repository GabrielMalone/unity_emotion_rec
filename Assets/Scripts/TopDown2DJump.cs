using System.Collections;
using UnityEngine;
// 1. ADD THIS LINE AT THE TOP
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
    public float jumpZoomMultiplier = 2.5f;
    private SpriteRenderer spriteRenderer; // so jumping object appears over every other object
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
        if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) && !isJumping)
        {
            StartCoroutine(PerformJumpArc());
        }
    }

    private IEnumerator PerformJumpArc()
    {
        isJumping = true;
        trail.emitting = false;
        spriteRenderer.sortingOrder = originalSortingOrder + sortingOrder; 
        Vector3 originalScale = spriteTransform.localScale;
        Vector2 travelDirection = rb.linearVelocity.normalized;
        float originalSpeed = rb.linearVelocity.magnitude;

        // Ignore collisions between the player’s layer and the Obstacles layer.
        int obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);
        if (obstacleLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, obstacleLayer, true);
        }

        //This will keep track of how long we’ve been jumping.
        float timer = 0f;
        Vector3 originalLocalPos = spriteTransform.localPosition;
        
        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            // This creates the arc.
            float progress = timer / jumpDuration;
            // Multiply something increasing by something decreasing --> progress * (1f-progress) this gives parabola
            float heightOffset = 1.2f * peakHeight * progress * (1f - progress);
            // put into air, but this is really just basically a forward to back movement of the player
            spriteTransform.localPosition = new Vector3(originalLocalPos.x + (heightOffset/2), originalLocalPos.y + heightOffset, originalLocalPos.z);
            // then actually scale the sprite
            float heightPercent = heightOffset / peakHeight;
            float scaleMultiplier = Mathf.Lerp(1f, jumpZoomMultiplier, heightPercent);
            spriteTransform.localScale = originalScale * scaleMultiplier;

            if (travelDirection != Vector2.zero)
            {
                rb.linearVelocity = travelDirection * (originalSpeed * airborneSpeedMultiplier);
            }
            // wait for next frame from deltaTime
            yield return null;
        }

        spriteTransform.localPosition = originalLocalPos;
        if (obstacleLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, obstacleLayer, false);
        }

        isJumping = false;
        trail.emitting = true;
        spriteRenderer.sortingOrder = originalSortingOrder;
        spriteTransform.localScale = originalScale;
    }
}
