using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] IObstacleController[] obstacles;

    private bool _isPlayerMoving;

    /// <summary>
    /// call this when player start to move
    /// </summary>
    /// <param name="isMoving"></param>
    private void OnPlayerMove(bool isMoving)
    {
        _isPlayerMoving = isMoving;
    }
}
