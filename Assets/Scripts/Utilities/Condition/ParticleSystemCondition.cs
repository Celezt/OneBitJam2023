using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemCondition : Condition
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    public override bool OnCondition() => !_particleSystem.isPlaying;
}
