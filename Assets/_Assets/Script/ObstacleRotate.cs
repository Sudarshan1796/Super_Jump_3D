using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRotate : MonoBehaviour
{
    [SerializeField] private float rotationAngle;
    [SerializeField] private RotationAxis axis;
    [SerializeField] private RotationDirection direction;

    private bool _isPlayerMoving;
    void Start()
    {
        Initialized();
    }
    void OnEnable()
    {
        GameUpdater.GetInstance.AddToUpdateEvent(UpdateMethod);
    }
    void OnDisable()
    {
        GameUpdater.GetInstance.RemoveFromUpdateEvent(UpdateMethod);
    }
    public void Initialized()
    {
        _isPlayerMoving = false;
    }
    /// <summary>
    /// call this when player start to move
    /// </summary>
    /// <param name="isMoving"></param>
    private void OnPlayerMove(bool isMoving)
    {
        _isPlayerMoving = isMoving;
    }
    void UpdateMethod()
    {
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

