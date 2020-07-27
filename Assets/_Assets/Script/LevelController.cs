using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<Transform> jumpPoints;

    public List<Transform> GetJumpPoints()
    {
        return jumpPoints;
    }
}
