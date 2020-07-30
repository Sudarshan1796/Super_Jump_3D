using com.SuperJump.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameVariables;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.6f,
                  slowFactor = 20,
                  jumpAngle = 45.0f,
                  gravity = 9.8f;

    private GamePlayManager gamePlayManager;
    private Transform jumpPoint;
    private Animator animator;
    private Rigidbody playerRigidBody;
    private LTDescr leanTweenObject;
    private Vector3 initialPosition;
    private Vector3 targetJumpPosition;
    private float slowMotionTimeScale;
    private bool isJumping;
    private bool isSlowMotionDone;

    private int idle = Animator.StringToHash("Idle");
    private int jump_1 = Animator.StringToHash("Jump_1");
    private int jump_2 = Animator.StringToHash("Jump_2");
    private int jump_3 = Animator.StringToHash("Jump_3");
    private int lose = Animator.StringToHash("Lose");
    private int win = Animator.StringToHash("Win");

    private static CharacterController instance;
    public static CharacterController GetInstance
    {
        get
        {
            if(instance==null)
            {
                instance = FindObjectOfType<CharacterController>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        gamePlayManager = GamePlayManager.GetInstance;
        playerRigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        slowMotionTimeScale = Time.timeScale / slowFactor;
        Init();
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

    public void Init()
    {
        transform.position = Vector3.zero;
        GetNextJumpingPoint();
        animator.SetTrigger(idle);
    }

    void UpdateMethod()
    {
        if (gamePlayManager.gamePlayState == GamePlayState.Playing)
        {
            if (!isJumping && Input.GetMouseButtonDown(0))
            {
                isJumping = true;
                initialPosition = transform.position;
                leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, speed).setEase(LeanTweenType.linear);//.setOnComplete(GetNextJumpingPoint);
                leanTweenObject.resume();
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
                gamePlayManager.Jump(true);
            }

            if (!isSlowMotionDone && isJumping && Vector3.Distance(transform.position, jumpPoint.position) <= Vector3.Distance(initialPosition, jumpPoint.position) * 0.6f)
            {
                isSlowMotionDone = true;
                Time.timeScale = slowMotionTimeScale;
                //Time.fixedDeltaTime /= slowFactor;
                //Time.maximumDeltaTime /= slowFactor;
                StartCoroutine(ResetTimeScale());
                leanTweenObject.pause();
                StartCoroutine(Jump());
            }
        }
    }

    IEnumerator Jump()
    {
        targetJumpPosition = transform.position + new Vector3(0, 0, 6);
        float target_Distance = Vector3.Distance(transform.position, targetJumpPosition);
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * jumpAngle * Mathf.Deg2Rad) / gravity);
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(jumpAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(jumpAngle * Mathf.Deg2Rad);
        float flightDuration = target_Distance / Vx;
        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
            if (elapse_time > flightDuration * 0.2f)
            {
                LeanTween.move(gameObject, jumpPoint.localPosition, speed * 0.5f).setEase(LeanTweenType.linear).setOnComplete(GetNextJumpingPoint);
                break;
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
            UIManager.GetInstance.OnPlayerJump(LevelManager.GetIntance.jumpPointIndex);
            transform.LookAt(jumpPoint);
        }
        else
        {
            gamePlayManager.OnGameOver(true);
            animator.SetTrigger(win);
        }
        gamePlayManager.Jump(false);
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
