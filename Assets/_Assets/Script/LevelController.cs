using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameVariables;

public class LevelController : MonoBehaviour
{
    [SerializeField] private ControlType controlType;
    [SerializeField] private List<Transform> jumpPoints;
    [SerializeField] private List<ParticleSystem> winParticleEffects;
    [SerializeField] private FinalDestructibleObject finalDestroyableObject;

    public ControlType GetLevelControlType()
    {
        return controlType;
    }

    public List<Transform> GetJumpPoints()
    {
        return jumpPoints;
    }

    public List<ParticleSystem> GetWinParticleEffects()
    {
        return winParticleEffects;
    }

    public void BlastFinalObject()
    {
        finalDestroyableObject.BlastFinalObject();
    }
}
