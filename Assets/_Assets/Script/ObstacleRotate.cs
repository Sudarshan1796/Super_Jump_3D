using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRotate : MonoBehaviour
{
    [SerializeField] private float rotationAngle;
    [SerializeField] private RotationAxis axis;
    [SerializeField] private RotationDirection direction;

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
            GameUpdater.GetInstance.AddToUpdateEvent(UpdateMethod);
        GamePlayManager.GetInstance.onFinishJump += OnJumpEnd;
        GamePlayManager.GetInstance.onStartJump += OnJumpStart;
    }

    void OnDisable()
    {
        RemoveListner();
    }

    private void RemoveListner()
    {
        if (GameUpdater.GetInstance)
            GameUpdater.GetInstance.RemoveFromUpdateEvent(UpdateMethod);
        GamePlayManager.GetInstance.onFinishJump -= OnJumpEnd;
        GamePlayManager.GetInstance.onStartJump -= OnJumpStart;
    }

    public void Initialized()
    {
        _isPlayerMoving = false;
        _gamePlayManager = GamePlayManager.GetInstance;
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
        if (axis == RotationAxis.Yaxis)
        {
            if(direction==RotationDirection.Positive)
            {
                transform.RotateAround(transform.position, Vector3.up, rotationAngle * Time.deltaTime);
            }
            else
            {
                transform.RotateAround(transform.position, Vector3.down, rotationAngle * Time.deltaTime);
            }
        }
        else if (axis == RotationAxis.Xaxis)
        {
            if (direction == RotationDirection.Positive)
            {
                transform.RotateAround(transform.position, Vector3.right, rotationAngle * Time.deltaTime);
            }
            else
            {
                transform.RotateAround(transform.position, Vector3.left, rotationAngle * Time.deltaTime);
            }
        }
    }
}
public enum RotationAxis
{
    Xaxis,
    Yaxis,
    Zaxis,
}
public enum RotationDirection
{
    Positive,
    Negative
}

