using com.SuperJump.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameVariables;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.6f,
                  jumpAngle = 45.0f,
                  gravity = 9.8f;
    [SerializeField] private SkinnedMeshRenderer characterMesh;
    [SerializeField] private GameObject dummyCharacterPrefab;
    [SerializeField] private GameObject ragDollPrefab;
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform dummyCharacterParent;
    [SerializeField] private List<Transform> dummyCharactersPositions;
    [SerializeField] private List<CharacterColor> characterColors;

    private GamePlayManager gamePlayManager;
    private Transform jumpPoint;
    private Animator animator;
    private Rigidbody playerRigidBody;
    private Quaternion initialCharcterBodyRotation;
    private LTDescr leanTweenObject;
    private Vector3 initialPosition;
    private Vector3 targetJumpPosition;
    private Coroutine resetTimeScaleCoroutine;
    private List<Animator> dummyCharactersAnimators;
    private List<Transform> dummyCharacters;
    private List<GameObject> ragDolls;
    private ControlType controlType;
    private int lastAddedColorIndex = -1;
    private int lastPlayedAnimationIndex = -1;
    private int dummyCharacterIndex = 0;
    private int dummyCharacterIntstantiatingCounter = 0;
    private bool isMoving;
    private bool isJumping;
    private bool isSlowMotionTriggered;
    private bool isInitialSlowMotionGoingOn;

    private readonly float oneTapSlowMotionMultiplier = 15;
    private readonly float twoTapSlowMotionMultiplier = 30;
    private readonly float jumpSlowMotionMultiplier = 0.125f;

    private readonly int idle   = Animator.StringToHash("Idle");
    private readonly int jump_1 = Animator.StringToHash("Jump_1");
    private readonly int jump_2 = Animator.StringToHash("Jump_2");
    private readonly int jump_3 = Animator.StringToHash("Jump_3");
    private readonly int jump_4 = Animator.StringToHash("Jump_4");
    private readonly int jump_5 = Animator.StringToHash("Jump_5");
    private readonly int jump_6 = Animator.StringToHash("Jump_6");
    private readonly int jump_7 = Animator.StringToHash("Jump_7");
    private readonly int jump_8 = Animator.StringToHash("Jump_8");
    private readonly int lose   = Animator.StringToHash("Lose");
    private readonly int win    = Animator.StringToHash("Win");

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
        gamePlayManager             = GamePlayManager.GetInstance;
        initialCharcterBodyRotation = characterBody.rotation;
        playerRigidBody             = GetComponent<Rigidbody>();
        animator                    = GetComponent<Animator>();
    }

    void Start()
    {
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
            PlayDummyCharacterAnimation(lose);
            gamePlayManager.OnGameOver(false);
        }
        else
        {
            if(isJumping)
            {
                animator.speed = 0;
                PlayDummyCharacterAnimation(0, 0, true);
                leanTweenObject.pause();
                gamePlayManager.OnGameOver(false);
            }
            else
            {
                leanTweenObject.pause();
                animator.speed = 0;
                PlayDummyCharacterAnimation(0, 0, true);
                gamePlayManager.OnGameOver(false);
            }
        }
    }

    public void Init(bool isRestart = false)
    {
        if(!isRestart)
        {
            SetCharacterColor();
        }

        gameObject.SetActive(true);
        characterBody.rotation = initialCharcterBodyRotation;
        DestroyDummyCharacters();
        DestroyRagDolls();
        if (dummyCharacters == null || dummyCharacters.Count == 0)
        {
            dummyCharacters = LevelManager.GetIntance.GetDummyCharactersInCurrentLevel();
        }
        animator.speed = 1;
        dummyCharacterIntstantiatingCounter = 0;
        transform.position = Vector3.zero;
        controlType = LevelManager.GetIntance.GetLevelControlType();
        isJumping = false;
        GetNextDestinationPoint();
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
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            isMoving = true;
            initialPosition = transform.position;
            leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, moveTime).setEase(LeanTweenType.linear).setOnComplete(GetNextDestinationPoint);
            PlayRandomJumpAnimation();
            gamePlayManager.Jump(true);
        }

        if (!isSlowMotionTriggered && isMoving && Vector3.Distance(transform.position, jumpPoint.position) <= Vector3.Distance(initialPosition, jumpPoint.position) * 0.6f)
        {
            isSlowMotionTriggered = true;
            StartLeanTweenSlowMotion(leanTweenObject,oneTapSlowMotionMultiplier, false);
            StartCoroutine(ResetTimeScale(0.6f));
        }
    }

    private void TwoTapControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isMoving)
            {
                isMoving = true;
                initialPosition = transform.position;
                leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, moveTime).setEase(LeanTweenType.linear);
                PlayInitialJumpAnimtionForTwoTap();
                gamePlayManager.Jump(true);
            }
            else if(isInitialSlowMotionGoingOn)
            {
                PlayRandomJumpAnimation();
                leanTweenObject.pause();
                StopCoroutine(resetTimeScaleCoroutine);
                isInitialSlowMotionGoingOn = false;
                StartTwoTapSlowMotion(false);
                StartCoroutine(Jump());
            }
        }

        if (!isSlowMotionTriggered && isMoving && Vector3.Distance(transform.position, jumpPoint.position) <= Vector3.Distance(initialPosition, jumpPoint.position) * 0.60f)
        {
            isSlowMotionTriggered = true;
            StartLeanTweenSlowMotion(leanTweenObject, twoTapSlowMotionMultiplier, false);
            resetTimeScaleCoroutine = StartCoroutine(ResetTimeScale(2));
        }
    }

    IEnumerator Jump()
    {
        isJumping = true;
        targetJumpPosition        = transform.position + new Vector3(0, 0, 3.5f);
        float target_Distance     = Vector3.Distance(transform.position, targetJumpPosition);
        float jumpVelocity        = target_Distance / (Mathf.Sin(2 * jumpAngle * Mathf.Deg2Rad) / gravity);
        float Vx                  = Mathf.Sqrt(jumpVelocity) * Mathf.Cos(jumpAngle * Mathf.Deg2Rad);
        float Vy                  = Mathf.Sqrt(jumpVelocity) * Mathf.Sin(jumpAngle * Mathf.Deg2Rad);
        float flightDuration      = target_Distance / Vx;
        float elapse_time         = 0;

        while (elapse_time < flightDuration && gamePlayManager.gamePlayState != GamePlayState.GameOver)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * jumpSlowMotionMultiplier * Time.deltaTime, Vx * jumpSlowMotionMultiplier * Time.deltaTime);

            elapse_time += Time.deltaTime * jumpSlowMotionMultiplier;

            yield return null;
            if (elapse_time > flightDuration * 0.275f)
            {
                leanTweenObject = LeanTween.move(gameObject, jumpPoint.localPosition, moveTime * 0.5f).setEase(LeanTweenType.linear).setOnComplete(GetNextDestinationPoint);
                StartTwoTapSlowMotion(true);
                isJumping = false;
                break;
            }
        }
    }

    private void GetNextDestinationPoint()
    {
        if (LevelManager.GetIntance.jumpPointIndex < LevelManager.GetIntance.GetTotalJumpPointsCount())
        {
            dummyCharacterIntstantiatingCounter += 1;
            if(dummyCharacterIntstantiatingCounter % 3 == 0)
            {
                ReParentDummyCharacter();
            }
            isMoving = false;
            isSlowMotionTriggered = false;
            jumpPoint = LevelManager.GetIntance.GetNextJumpPoint();
            LevelManager.GetIntance.jumpPointIndex++;
            UIManager.GetInstance.OnPlayerJump(LevelManager.GetIntance.jumpPointIndex);
            transform.LookAt(jumpPoint);
        }
        else
        {
            LevelManager.GetIntance.BlastFinalDestroyableObject();
            gamePlayManager.OnGameOver(true);
            animator.SetTrigger(win);
            PlayDummyCharacterAnimation(win);
            Celebrate();
        }
        gamePlayManager.Jump(false);
    }
  
    private void StartLeanTweenSlowMotion(LTDescr leanTweenObj, float slowMotionMultiplier, bool shouldReset)
    {
        if (!shouldReset)
        {
            leanTweenObj.setTime(leanTweenObj.time * slowMotionMultiplier);
            SetAnimatorSpeed(animator.speed / slowMotionMultiplier);
        }
        else
        {
            leanTweenObj.setTime(leanTweenObj.time / slowMotionMultiplier);
            SetAnimatorSpeed(1);
        }
    }

    private void StartTwoTapSlowMotion(bool shouldReset)
    {
        if (!shouldReset)
        {
            SetAnimatorSpeed(animator.speed * (twoTapSlowMotionMultiplier * 0.15f));
        }
        else
        {
            SetAnimatorSpeed(1);
        }
    }

    private IEnumerator ResetTimeScale(float resetTime)
    {
        isInitialSlowMotionGoingOn = true;
        yield return new WaitForSecondsRealtime(resetTime);
        leanTweenObject.setOnComplete(GetNextDestinationPoint);
        if (controlType == ControlType.OneTap)
        {
            StartLeanTweenSlowMotion(leanTweenObject,oneTapSlowMotionMultiplier, true);
        }
        else
        {
            StartLeanTweenSlowMotion(leanTweenObject,twoTapSlowMotionMultiplier, true);
        }
        isInitialSlowMotionGoingOn = false;
    }

    private void SetCharacterColor()
    {
        lastAddedColorIndex = GetRandomColorIndex();
        characterMesh.sharedMaterial.color = characterColors[lastAddedColorIndex].color;

        int GetRandomColorIndex()
        {
            if (lastAddedColorIndex == -1)
            {
                return Random.Range(0, characterColors.Count);
            }
            else
            {
                var val = Random.Range(0, characterColors.Count);
                if (val == lastAddedColorIndex)
                {
                    if (val < characterColors.Count - 1)
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

    private void SetAnimatorSpeed(float speed)
    {
        animator.speed = speed;
        if(dummyCharactersAnimators != null && dummyCharactersAnimators.Count > 0)
        {
            foreach(var animator in dummyCharactersAnimators)
            {
                animator.speed = speed;
            }
        }
    }

    private void PlayInitialJumpAnimtionForTwoTap()
    {
        animator.SetTrigger(jump_3);
        PlayDummyCharacterAnimation(jump_3);
        lastPlayedAnimationIndex = 2;
    }

    private void PlayRandomJumpAnimation()
    {
        lastPlayedAnimationIndex = GetRandomAnimationIndex();
        switch (lastPlayedAnimationIndex)
        {
            case 0:
                animator.SetTrigger(jump_1);
                PlayDummyCharacterAnimation(jump_1);
                break;
            case 1:
                animator.SetTrigger(jump_2);
                PlayDummyCharacterAnimation(jump_2);
                break;
            case 2:
                animator.SetTrigger(jump_3);
                PlayDummyCharacterAnimation(jump_3);
                break;
            case 3:
                animator.SetTrigger(jump_4);
                PlayDummyCharacterAnimation(jump_4);
                break;
            case 4:
                animator.SetTrigger(jump_5);
                PlayDummyCharacterAnimation(jump_5);
                break;
            case 5:
                animator.SetTrigger(jump_6);
                PlayDummyCharacterAnimation(jump_6);
                break;
            case 6:
                animator.SetTrigger(jump_7);
                PlayDummyCharacterAnimation(jump_7);
                break;
            case 7:
                animator.SetTrigger(jump_8);
                PlayDummyCharacterAnimation(jump_8);
                break;
        }

        int GetRandomAnimationIndex()
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
    }

    private void PlayDummyCharacterAnimation(int val, float speed = 1, bool shouldSetSpeed = false)
    {
        if (dummyCharactersAnimators != null && dummyCharactersAnimators.Count > 0)
        {
            if (!shouldSetSpeed)
            {
                foreach (var anim in dummyCharactersAnimators)
                {
                    anim.SetTrigger(val);
                }
            }
            else
            {
                foreach (var anim in dummyCharactersAnimators)
                {
                    anim.speed = speed;
                }
            }
        }
    }

    private void ReParentDummyCharacter()
    {
        if(dummyCharactersAnimators == null)
        {
            dummyCharactersAnimators = new List<Animator>();
        }

        var dummyCharacterRotation = dummyCharacters[dummyCharacterIndex].localRotation;
        dummyCharacters[dummyCharacterIndex].SetParent(dummyCharacterParent);
        dummyCharacters[dummyCharacterIndex].localRotation = dummyCharacterRotation;
        dummyCharacters[dummyCharacterIndex].localPosition = dummyCharactersPositions[dummyCharacterIndex].localPosition;
        dummyCharactersAnimators.Add(dummyCharacters[dummyCharacterIndex].GetComponent<Animator>());
        dummyCharacterIndex++;
    }

    private void Celebrate()
    {
        var camPos = Camera.main.transform.position;
        camPos.y = 0;
        characterBody.LookAt(camPos);

        for (int i = 0; i < dummyCharacters.Count; i++)
        {
            LeanTween.moveLocal(dummyCharacters[i].gameObject, dummyCharactersPositions[i].localPosition * 3, 0.5f).setEase(LeanTweenType.linear);
        }
    }

    internal void InstantiateRagDolls()
    {
        if(ragDolls == null)
        {
            ragDolls = new List<GameObject>();
        }

        var ragdoll = Instantiate(ragDollPrefab);
        ragdoll.transform.position = transform.position;
        ragdoll.transform.rotation = transform.rotation;
        gameObject.SetActive(false);
        ragdoll.SetActive(true);
        ragDolls.Add(ragdoll);

        if(dummyCharacterIndex > 0)
        {
            foreach(var dummyCharacter in dummyCharacters)
            {
                var ragdol = Instantiate(ragDollPrefab);
                ragdol.transform.position = dummyCharacter.position;
                ragdol.transform.rotation = dummyCharacter.rotation;
                ragdol.SetActive(false);
                ragdol.SetActive(true);
                ragDolls.Add(ragdol);
            }
        }
    }

    private void DestroyRagDolls()
    {
        if (ragDolls != null && ragDolls.Count > 0)
        {
            foreach (var ragDoll in ragDolls)
            {
                Destroy(ragDoll);
            }
            ragDolls.Clear();
        }
    }

    private void DestroyDummyCharacters()
    {
        if (dummyCharacters != null && dummyCharacters.Count > 0)
        {
            foreach (var dummyCharacter in dummyCharacters)
            {
                Destroy(dummyCharacter.gameObject);
            }

            dummyCharacters.Clear();
            if(dummyCharactersAnimators != null)
            dummyCharactersAnimators.Clear();
            dummyCharacterIndex = 0;
        }
    }
}
