using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", playerController.CurrentSpeed);
        animator.SetBool("IsGrounded", playerController.IsGrounded);
        animator.SetBool("IsRunning", playerController.IsRunning);

        if (playerController.JumpTriggered)
        {
            animator.SetTrigger("Jump");
            playerController.ResetJumpTrigger();
        }
    }
}