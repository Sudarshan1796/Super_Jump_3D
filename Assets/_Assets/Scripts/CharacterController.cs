using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameVariables;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.6f,
                  slowFactor = 20;

    private GamePlayManager gamePlayManager;
    private Transform jumpPoint;
    private Animator animator;
    private Rigidbody playerRigidBody;
    private LTDescr leanTweenObject;
    private Vector3 initialPosition;
    private float slowMotionTimeScale;
    private bool isJumping;
    private bool isSlowMotionDone;

    private int idle = Animator.StringToHash("Idle");
    private int jump_1 = Animator.StringToHash("Jump_1");
    private int jump_2 = Animator.StringToHash("Jump_2");
    private int jump_3 = Animator.StringToHash("Jump_3");
    private int lose = Animator.StringToHash("Lose");
    private int win = Animator.StringToHash("Win");

    private void Awake()
    {
        gamePlayManager = GamePlayManager.GetInstance;
        playerRigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        slowMotionTimeScale = Time.timeScale / slowFactor;
        GetNextJumpingPoint();
        animator.SetTrigger(idle);
    }

    private void OnEnable()
    {
        if (GameUpdater.GetInstance)
            GameUpdater.GetInstance.AddToUpdateEvent(UpdateMethod);
    }

    private void OnDisable()
    {
        if (GameUpdater.GetInstance)
            GameUpdater.GetInstance.RemoveFromUpdateEvent(UpdateMethod);
    }

    void UpdateMethod()
    {
        if (gamePlayManager.gamePlayState == GamePlayState.GameStart)
        {
            if (!isJumping && Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                initialPosition = transform.position;
                leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, speed).setEase(LeanTweenType.linear).setOnComplete(GetNextJumpingPoint);
                var val = Random.Range(0, 3);
                switch (val)
                {
                    case 0:
                        animator.SetTrigger(jump_1);
                        break;
                    case 1:
                        animator.SetTrigger(jump_2);
                        break;
                    case 2:
                        animator.SetTrigger(jump_3);
                        break;
                }
            }

            if (!isSlowMotionDone && isJumping && Vector3.Distance(transform.position, jumpPoint.position) <= Vector3.Distance(initialPosition, jumpPoint.position) * 0.6f)
            {
                isSlowMotionDone = true;
                Time.timeScale = slowMotionTimeScale;
                //Time.fixedDeltaTime /= slowFactor;
                //Time.maximumDeltaTime /= slowFactor;
                StartCoroutine(ResetTimeScale());
            }
        }
    }
    
    private void GetNextJumpingPoint()
    {
        if (LevelManager.GetIntance.jumpPointIndex < LevelManager.GetIntance.GetTotalJumpPointsCount())
        {
            isJumping = false;
            isSlowMotionDone = false;
            jumpPoint = LevelManager.GetIntance.GetNextJumpPoint();
            LevelManager.GetIntance.jumpPointIndex++;
            transform.LookAt(jumpPoint);
        }
        else
        {
            gamePlayManager.OnGameOver(true);
            animator.SetTrigger(win);
        }
    }

    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        //Time.fixedDeltaTime *= slowFactor;
        //Time.maximumDeltaTime *= slowFactor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        leanTweenObject.pause();
        animator.SetTrigger(lose);
        gamePlayManager.OnGameOver(false);
    }
}
