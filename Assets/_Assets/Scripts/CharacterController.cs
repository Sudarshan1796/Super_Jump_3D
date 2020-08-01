using com.SuperJump.UI;
using System.Collections;
using UnityEngine;
using static GameVariables;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.6f,
                  slowFactor = 20,
                  jumpAngle = 45.0f,
                  gravity = 9.8f,
                  jumpTimeScaleFactor = 0.3f;

    private GamePlayManager gamePlayManager;
    private Transform jumpPoint;
    private Animator animator;
    private Rigidbody playerRigidBody;
    private LTDescr leanTweenObject;
    private Vector3 initialPosition;
    private Vector3 targetJumpPosition;
    private Coroutine resetTimeScaleCoroutine;
    private ControlType controlType;
    private float initialSlowMotionTimeScale;
    private float finalSlowMotionTimeScale;
    private int lastPlayedAnimationIndex = -1;
    private bool isJumping;
    private bool isSlowMotionTriggered;
    private bool isInitialSlowMotionGoingOn;

    private readonly int idle = Animator.StringToHash("Idle");
    private readonly int jump_1 = Animator.StringToHash("Jump_1");
    private readonly int jump_2 = Animator.StringToHash("Jump_2");
    private readonly int jump_3 = Animator.StringToHash("Jump_3");
    private readonly int jump_4 = Animator.StringToHash("Jump_4");
    private readonly int jump_5 = Animator.StringToHash("Jump_5");
    private readonly int jump_6 = Animator.StringToHash("Jump_6");
    private readonly int jump_7 = Animator.StringToHash("Jump_7");
    private readonly int jump_8 = Animator.StringToHash("Jump_8");
    private readonly int lose = Animator.StringToHash("Lose");
    private readonly int win = Animator.StringToHash("Win");

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
        initialSlowMotionTimeScale = Time.timeScale / slowFactor;
        finalSlowMotionTimeScale = Time.timeScale / (slowFactor * jumpTimeScaleFactor);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (controlType == ControlType.OneTap)
        {
            leanTweenObject.pause();
            animator.SetTrigger(lose);
            gamePlayManager.OnGameOver(false);
        }
        else
        {
            if(isJumping)
            {
                StopAllCoroutines();
                SetTimeScale(0);
                gamePlayManager.OnGameOver(false);
            }
            else
            {
                leanTweenObject.pause();
                animator.SetTrigger(lose);
                gamePlayManager.OnGameOver(false);
            }
        }
    }

    public void Init()
    {
        SetTimeScale(1);
        transform.position = Vector3.zero;
        controlType = LevelManager.GetIntance.GetLevelControlType();
        GetNextJumpingPoint();
        animator.SetTrigger(idle);
    }

    void UpdateMethod()
    {
        if (gamePlayManager.gamePlayState == GamePlayState.Playing)
        {
            if(controlType == ControlType.OneTap)
            {
                OneTapControl();
            }
            else
            {
                TwoTapControl();
            }
        }
    }

    private void OneTapControl()
    {
        if (!isJumping && Input.GetMouseButtonDown(0))
        {
            isJumping = true;
            initialPosition = transform.position;
            leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, speed).setEase(LeanTweenType.linear).setOnComplete(GetNextJumpingPoint);
            PlayRandomJumpAnimation();
            gamePlayManager.Jump(true);
        }

        if (!isSlowMotionTriggered && isJumping && Vector3.Distance(transform.position, jumpPoint.position) <= Vector3.Distance(initialPosition, jumpPoint.position) * 0.6f)
        {
            isSlowMotionTriggered = true;
            SetTimeScale(initialSlowMotionTimeScale);
            //Time.fixedDeltaTime /= slowFactor;
            //Time.maximumDeltaTime /= slowFactor;
            StartCoroutine(ResetTimeScale(0.5f));
        }
    }

    private void TwoTapControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isJumping)
            {
                isJumping = true;
                initialPosition = transform.position;
                leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, speed).setEase(LeanTweenType.linear);
                leanTweenObject.resume();
                PlayInitialJumpAnimtionForTwoTap();
                gamePlayManager.Jump(true);
            }
            else if(isInitialSlowMotionGoingOn)
            {
                PlayRandomJumpAnimation();
                leanTweenObject.pause();
                StopCoroutine(resetTimeScaleCoroutine);
                isInitialSlowMotionGoingOn = false;
                SetTimeScale(finalSlowMotionTimeScale);
                StartCoroutine(Jump());
            }
        }

        if (!isSlowMotionTriggered && isJumping && Vector3.Distance(transform.position, jumpPoint.position) <= Vector3.Distance(initialPosition, jumpPoint.position) * 0.65f)
        {
            isSlowMotionTriggered = true;
            SetTimeScale(initialSlowMotionTimeScale * 0.5f);
            resetTimeScaleCoroutine = StartCoroutine(ResetTimeScale(2));
        }
    }

    IEnumerator Jump()
    {
        targetJumpPosition        = transform.position + new Vector3(0, 0, 5);
        float target_Distance     = Vector3.Distance(transform.position, targetJumpPosition);
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * jumpAngle * Mathf.Deg2Rad) / gravity);
        float Vx                  = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(jumpAngle * Mathf.Deg2Rad);
        float Vy                  = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(jumpAngle * Mathf.Deg2Rad);
        float flightDuration      = target_Distance / Vx;
        float elapse_time         = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
            if (elapse_time > flightDuration * 0.3f)
            {
                LeanTween.move(gameObject, jumpPoint.localPosition, speed * 0.5f).setEase(LeanTweenType.linear).setOnComplete(GetNextJumpingPoint);
                SetTimeScale(1);
                break;
            }
        }
    }

    private void GetNextJumpingPoint()
    {
        if (LevelManager.GetIntance.jumpPointIndex < LevelManager.GetIntance.GetTotalJumpPointsCount())
         {
            isJumping = false;
            isSlowMotionTriggered = false;
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

    private void SetTimeScale(float val)
    {
        Time.timeScale = val;
    }

    private IEnumerator ResetTimeScale(float resetTime)
    {
        isInitialSlowMotionGoingOn = true;
        yield return new WaitForSecondsRealtime(resetTime);
        leanTweenObject.setOnComplete(GetNextJumpingPoint);
        SetTimeScale(1);
        //Time.fixedDeltaTime *= slowFactor;
        //Time.maximumDeltaTime *= slowFactor;
        isInitialSlowMotionGoingOn = false;
    }

    private void PlayInitialJumpAnimtionForTwoTap()
    {
        animator.SetTrigger(jump_3);
    }

    private void PlayRandomJumpAnimation()
    {
        lastPlayedAnimationIndex = GetRandomAnimationIndex();
        switch (lastPlayedAnimationIndex)
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
            case 3:
                animator.SetTrigger(jump_4);
                break;
            case 4:
                animator.SetTrigger(jump_5);
                break;
            case 5:
                animator.SetTrigger(jump_6);
                break;
            case 6:
                animator.SetTrigger(jump_7);
                break;
            case 7:
                animator.SetTrigger(jump_8);
                break;
        }

        int GetRandomAnimationIndex()
        {
            if (controlType == ControlType.OneTap)
            {
                if (lastPlayedAnimationIndex == -1)
                {
                    return Random.Range(0, 7);
                }
                else
                {
                    var val = Random.Range(0, 7);
                    if (val == lastPlayedAnimationIndex)
                    {
                        if (val < 7)
                        {
                            return val + 1;
                        }
                        else
                        {
                            return val - 1;
                        }
                    }
                    else
                    {
                        return val;
                    }
                }
            }
            else
            {
                var val = Random.Range(0, 7);
                if (val == lastPlayedAnimationIndex || val == 3)
                {
                    if (val < 7)
                    {
                        return val + 1;
                    }
                    else
                    {
                        return val - 1;
                    }
                }
                else
                {
                    return val;
                }
            }
        }
    }
}
