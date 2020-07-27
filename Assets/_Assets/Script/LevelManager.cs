using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<GameObject> levelJumpPoints;
    public int jumpPointIndex;
    public List<GameObject> levels;

    private static LevelManager instance;
    public static LevelManager GetIntance
    {
        get
        {
            if(instance==null)
            {
                instance = FindObjectOfType(typeof(LevelManager)) as LevelManager;
            }
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
        levelJumpPoints = new List<GameObject>();
    }
    public void InitiliseJumpPoints(LevelController levelController)
    {
        jumpPointIndex = 0;
        levelJumpPoints.Clear();
        levelJumpPoints = levelController.GetJumpPoints();
    }
    public Vector3 GetNextJumpPoint()
    {
        return levelJumpPoints[jumpPointIndex].transform.localPosition;
    }
    private void LoadLevel(int level)
    {
        int index = level % levels.Count;
        var levelObject = Instantiate(levels[index],Vector3.zero, Quaternion.identity);
        LevelController levelController = levelObject.GetComponent<LevelController>();
        InitiliseJumpPoints(levelController);
    }
}
