
using UnityEngine;

public static class GameVariables
{
    public enum GamePlayState : short
    {
        None,
        GameStart,
        Playing,
        GameOver
    }

    public enum ControlType : short
    {
        OneTap,
        TwoTap
    }

    [System.Serializable]
    public class CharacterColor
    {
        public Color color;
    }
}
