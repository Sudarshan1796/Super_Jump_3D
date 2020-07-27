using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> jumpPoints;

    public List<GameObject> GetJumpPoints()
    {
        return jumpPoints;
    }
}
