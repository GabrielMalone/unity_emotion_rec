using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Start the current animation at a random point from 0% to 100%
        animator.Play(0, 0, Random.value);
    }
}