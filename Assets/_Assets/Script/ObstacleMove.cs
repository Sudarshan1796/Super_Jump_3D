﻿using System.Collections;
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
        transform.localPosition = initialPosition;
        targetPosition = finalPosition;
        isMovingToFinalPosition = true;
        _isPlayerMoving = false;
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
    private void OnPlayerMove(bool isMoving)
    {
        _isPlayerMoving = isMoving;
    }
}
