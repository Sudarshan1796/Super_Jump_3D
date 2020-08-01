using UnityEngine;

public class ObstacleAnimate : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        if (GamePlayManager.GetInstance != null)
        {
            GamePlayManager.GetInstance.onFinishJump += OnJumpEnd;
            GamePlayManager.GetInstance.onStartJump += OnJumpStart;
        }
    }

    private void OnDisable()
    {
        if (GamePlayManager.GetInstance != null)
        {
            GamePlayManager.GetInstance.onFinishJump -= OnJumpEnd;
            GamePlayManager.GetInstance.onStartJump -= OnJumpStart;
        }
    }

    private void OnJumpStart()
    {
        animator.speed = 0.0f;
    }

    private void OnJumpEnd()
    {
        animator.speed = 1.0f;
    }
}
