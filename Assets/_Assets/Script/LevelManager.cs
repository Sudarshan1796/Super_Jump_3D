using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int currentLevel;
    public int jumpPointIndex;
    public List<GameObject> levels;

    private List<Transform> levelJumpPoints;
    private GameObject currentLoadedevel;

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
        levelJumpPoints = new List<Transform>();
        currentLevel = PlayerPrefs.GetInt("Level", 1);
    }

    private void OnEnable()
    {
        if (GamePlayManager.GetInstance)
            GamePlayManager.GetInstance.onGamWon += IncreaseLevel;
    }

    private void OnDisable()
    {
        if (GamePlayManager.GetInstance)
            GamePlayManager.GetInstance.onGamWon -= IncreaseLevel;
    }

    public void InitiliseJumpPoints(LevelController levelController)
    {
        jumpPointIndex = 0;
        levelJumpPoints.Clear();
        levelJumpPoints = levelController.GetJumpPoints();
    }

    public Transform GetNextJumpPoint()
    {
        return levelJumpPoints[jumpPointIndex];
    }

    public int GetTotalJumpPointsCount()
    {
        return levelJumpPoints.Count;
    }

    public void LoadLevel()
    {
        if(currentLoadedevel != null)
        {
            Destroy(currentLoadedevel);
            currentLoadedevel = null;
        }
        int index = currentLevel % levels.Count;
        currentLoadedevel = Instantiate(levels[index], Vector3.zero, Quaternion.identity);
        LevelController levelController = currentLoadedevel.GetComponent<LevelController>();
        InitiliseJumpPoints(levelController);
        currentLoadedevel.SetActive(true);
    }

    private void IncreaseLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("Level", currentLevel);
    }
}
