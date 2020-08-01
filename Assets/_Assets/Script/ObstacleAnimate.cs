using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ObstacleAnimate : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void OnEnable()
    {
        GamePlayManager.GetInstance.onFinishJump += OnJumpEnd;
        GamePlayManager.GetInstance.onStartJump += OnJumpStart;
    }
    private void OnDisable()
    {
        GamePlayManager.GetInstance.onFinishJump -= OnJumpEnd;
        GamePlayManager.GetInstance.onStartJump -= OnJumpStart;
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
