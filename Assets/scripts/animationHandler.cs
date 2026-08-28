using UnityEngine;

public class animationHandler : MonoBehaviour
{
    public Animator animator;
    public string animationName;
    public void PlayAnimationFromAnimator()
    {
        animator.Play(animationName);
    }
}
