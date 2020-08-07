
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
//using GameAnalyticsSDK;
public class SDK_Initialiser : MonoBehaviour
{
    static SDK_Initialiser Instance { get; set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFB();
        }
    }


    void InitializeFB()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(FBInitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
    private void FBInitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isUnityShown)
    {
    }


    // Start is called before the first frame update
    //void Start()
    //{
    //    if (Application.isEditor == false)
    //    {
    //        GameAnalytics.Initialize();
    //    }
    //}
    //internal static void LevelStartEvent(int level)
    //{
    //    if (Application.isEditor == false)
    //    {
    //        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, level.ToString());
    //        GameEvents.StartLevelEvent(level);
    //    }
    //    print("Start : Level_" + level);
    //}

    //internal static void LevelSuccessEvent(int level)
    //{
    //    if (Application.isEditor == false)
    //    {
    //        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, level.ToString());
    //        GameEvents.WinEvent(level, 100) ;
    //    }
    //    print("Success : Level_" + level);
    //}
    //internal static void LevelFailEvent(int level, int round)
    //{
    //    if (Application.isEditor == false)
    //    {
    //        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, level.ToString(), round.ToString());
    //    }
    //    print("Fail : Level_" + level );
    //}
}
