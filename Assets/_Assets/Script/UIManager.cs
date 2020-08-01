using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.SuperJump.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> screens;
        [SerializeField] private GameObject winPanel, loosePanel;
        [SerializeField] private Button tapToStartBtn, retryBtn, nextBtn;
        [SerializeField] private LevelProgress levelProgress;
        [SerializeField] private Text curLvltext, nextLvlText;
        private static UIManager instance;
        public static UIManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(UIManager)) as UIManager;
                }
                return instance;
            }
        }
        private void Awake()
        {
            instance = this;
        }
        public void Activate(Screen screen)
        {
            for (int i = 0; i < screens.Count; i++)
            {
                if (screen.GetHashCode() == i)
                {
                    screens[i].SetActive(true);
                }
            }
        }
        public void Deactivate(Screen screen)
        {
            for (int i = 0; i < screens.Count; i++)
            {
                if (screen.GetHashCode() == i)
                {
                    screens[i].SetActive(false);
                }
            }
        }
        private void OnEnable()
        {
            AddListener();
            Activate(Screen.Home);
        }

        private void OnDisable()
        {
            RemoveListener();
        }

        private void AddListener()
        {
            tapToStartBtn.onClick.AddListener(OnTapToPlayClick);
            nextBtn.onClick.AddListener(OnNextLevelClick);
            retryBtn.onClick.AddListener(OnRetryClick);
            GamePlayManager.GetInstance.onGameLose += OnGameLoose;
            GamePlayManager.GetInstance.onGamWon += OnGameWin;
        }

        private void RemoveListener()
        {
            tapToStartBtn.onClick.RemoveListener(OnTapToPlayClick);
            nextBtn.onClick.RemoveListener(OnNextLevelClick);
            retryBtn.onClick.RemoveListener(OnRetryClick);
            if (GamePlayManager.GetInstance)
            {
                GamePlayManager.GetInstance.onGameLose -= OnGameLoose;
                GamePlayManager.GetInstance.onGamWon -= OnGameWin;
            }
        }

        private void OnTapToPlayClick()
        {
            Activate(Screen.Gameplay);
            Deactivate(Screen.Home);
            levelProgress.StartLevel();
            SetLevelText();
            GamePlayManager.GetInstance.gamePlayState = GameVariables.GamePlayState.Playing;
        }

        private void SetLevelText()
        {
            var count = LevelManager.GetIntance.currentLevel;
            curLvltext.text = count.ToString();
            nextLvlText.text = (count + 1).ToString();
        }

        private void OnRetryClick()
        {
            GamePlayManager.GetInstance.RestartGame();
            Activate(Screen.Gameplay);
            Deactivate(Screen.Results);
            levelProgress.StartLevel();
            SetLevelText();
            CharacterController.GetInstance.Init(true);
            GamePlayManager.GetInstance.gamePlayState = GameVariables.GamePlayState.Playing;
        }

        private void OnNextLevelClick()
        {
            GamePlayManager.GetInstance.RestartGame();
            Activate(Screen.Gameplay);
            Deactivate(Screen.Results);
            levelProgress.StartLevel();
            SetLevelText();
            CharacterController.GetInstance.Init();
            GamePlayManager.GetInstance.gamePlayState = GameVariables.GamePlayState.Playing;
        }

        private void OnGameLoose()
        {
            //GameUpdater.GetInstance.RemoveAllUpdate();
            GamePlayManager.GetInstance.gamePlayState = GameVariables.GamePlayState.GameOver;
            levelProgress.StopLevelprogress();
            Activate(Screen.Results);
            Deactivate(Screen.Gameplay);
            loosePanel.SetActive(true);
            winPanel.SetActive(false);
        }

        private void OnGameWin()
        {
            //GameUpdater.GetInstance.RemoveAllUpdate();
            GamePlayManager.GetInstance.gamePlayState = GameVariables.GamePlayState.GameOver;
            levelProgress.StopLevelprogress();
            Activate(Screen.Results);
            Deactivate(Screen.Gameplay);
            nextBtn.gameObject.SetActive(false);
            winPanel.SetActive(true);
            loosePanel.SetActive(false);
            Invoke(nameof(EnableNextButton), 4.0f);
        }

        public void OnPlayerJump(int value)
        {
            levelProgress.minValue = value - 1;
        }

        private void EnableNextButton()
        {
            nextBtn.gameObject.SetActive(true);
        }
    }

    public enum Screen
    {
        Loading,
        Home,
        Gameplay,
        Results
    }
}
