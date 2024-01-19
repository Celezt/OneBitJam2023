using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemHandler : Condition
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    public override bool OnCondition() => !_particleSystem.isPlaying;

    public void SetDuration(float duration)
    {
        var main = _particleSystem.main;
        main.duration = duration;
    }

    public void SetStartLifetime(float duration)
    {
        var main = _particleSystem.main;
        main.startLifetime = duration;
    }
}
