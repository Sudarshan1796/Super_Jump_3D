using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<Transform> jumpPoints;
    [SerializeField] private List<ParticleSystem> winParticleEffects;

    public List<Transform> GetJumpPoints()
    {
        return jumpPoints;
    }

    public List<ParticleSystem> GetWinParticleEffects()
    {
        return winParticleEffects;
    }
}
