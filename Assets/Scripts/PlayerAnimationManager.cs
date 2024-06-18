using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayJumpAnimation()
    {
        anim.SetBool("Jumping", true);
    }

    public void PlaySlideAnimation()
    {
        anim.SetTrigger("Slide");
    }

    public void StopJumpAnimation()
    {
        anim.SetBool("Jumping", false);
    }
}