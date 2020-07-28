using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMove : MonoBehaviour, IObstacleController
{
    [SerializeField]private Vector3 initialPosition;
    [SerializeField]private Vector3 finalPosition;
    [SerializeField] private float speed;

    private Vector3 targetPosition;
    private bool isMovingToFinalPosition;
    private bool _isPlayerMoving;
    private GamePlayManager _gamePlayManager;

    void Start()
    {
        Initialized();
    }

    void OnEnable()
    {
        AddListners();
    }

    private void AddListners()
    {
        if (GameUpdater.GetInstance)
        {
            GameUpdater.GetInstance.AddToUpdateEvent(UpdateMethod);
        }

        if (GamePlayManager.GetInstance)
        {
            GamePlayManager.GetInstance.onFinishJump += OnJumpEnd;
            GamePlayManager.GetInstance.onStartJump += OnJumpStart;
        }
    }

    void OnDisable()
    {
        RemoveListner();
    }

    private void RemoveListner()
    {
        if (GameUpdater.GetInstance)
        {
            GameUpdater.GetInstance.RemoveFromUpdateEvent(UpdateMethod);
        }

        if (GamePlayManager.GetInstance)
        {
            GamePlayManager.GetInstance.onFinishJump -= OnJumpEnd;
            GamePlayManager.GetInstance.onStartJump -= OnJumpStart;
        }
    }

    public void Initialized()
    {
        transform.localPosition = initialPosition;
        targetPosition = finalPosition;
        isMovingToFinalPosition = true;
        _isPlayerMoving = false;
        _gamePlayManager = GamePlayManager.GetInstance;
    }

    void UpdateMethod()
    {
        if (_gamePlayManager == null)
        {
            _gamePlayManager = GamePlayManager.GetInstance;
        }
        if (!_isPlayerMoving)
        {
            Activate();
        }
    }

    public void Activate()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, Time.deltaTime * speed);
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.1)
        {
            OnFinalPositionReach();
        }
    }

    public void OnFinalPositionReach()
    {
        if(isMovingToFinalPosition)
        {
            transform.localPosition = finalPosition;
            targetPosition = initialPosition;
            isMovingToFinalPosition = false;
        }
        else
        {
            transform.localPosition = initialPosition;
            targetPosition = finalPosition;
            isMovingToFinalPosition = true;
        }
    }

    /// <summary>
    /// call this when player start to move
    /// </summary>
    /// <param name="isMoving"></param>
    private void OnJumpStart()
    {
        _isPlayerMoving = true;
    }

    /// <summary>
    /// call this when player Finish to move
    /// </summary>
    /// <param name="isMoving"></param>
    private void OnJumpEnd()
    {
        _isPlayerMoving = false;
    }
}
