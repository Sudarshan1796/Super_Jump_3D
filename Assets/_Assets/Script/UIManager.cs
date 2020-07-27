using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.SuperJump.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> screens;
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
    }
    public enum Screen
    {
        Loading,
        Home,
        Gameplay
    }
}
