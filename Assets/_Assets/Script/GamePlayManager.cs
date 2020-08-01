using System;
using System.Collections;
using UnityEngine;
using static GameVariables;

public class GamePlayManager : MonoBehaviour
{
    private static GamePlayManager instance;
    internal static GamePlayManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GamePlayManager>();
            }
            return instance;
        }
    }

    internal GamePlayState gamePlayState;
    internal Action onGamWon;
    internal Action onGameLose;
    internal Action gameStart;
    internal Action onStartJump;
    internal Action onFinishJump;

    private void Awake()
    {
        if (!Application.isEditor)
        {
            Application.targetFrameRate = 60;
        }
    }

    private void Start()
    {
        EnableVibration(true);
        InitializeLevel();
    }

    internal void InitializeLevel()
    {
        LevelManager.GetIntance.LoadLevel();
        StartRun();
    }

    internal void StartRun()
    {
        gamePlayState = GamePlayState.GameStart;
    }

    internal void RestartGame()
    {
        InitializeLevel();
        StartRun();
    }

    internal void Jump(bool isStart)
    {
        if(isStart)
        {
            onStartJump?.Invoke();
        }
        else
        {
            onFinishJump?.Invoke();
        }
    }

    internal void OnGameOver(bool isWon)
    {
        gamePlayState = GamePlayState.GameOver;
        if (isWon)
        {
            //SDK_Initialiser.LevelSuccessEvent(ScoreAndLevelManager.GetInstance.GetGameLevel());
            onGamWon?.Invoke();
            StartCoroutine(LevelManager.GetIntance.PlayWinParticleEffects());
        }
        else
        {
            //SDK_Initialiser.LevelFailEvent(ScoreAndLevelManager.GetInstance.GetGameLevel(), 0, UIManager.GetInstance.GetPlayerProgress());
            onGameLose?.Invoke();
        }
    }

    private void EnableVibration(bool value)
    {
        HapticFeedback.SetVibrationOn(value);
    }

    internal void Vibrate(UIFeedbackType vibrationType)
    {
        HapticFeedback.Vibrate(vibrationType);
    }
}

