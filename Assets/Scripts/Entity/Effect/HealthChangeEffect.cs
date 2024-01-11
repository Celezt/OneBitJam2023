using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthChangeEffect : IEffect
{
    public float ValueChange = -10;

    public void Effect(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        if (!effector.GameObject.TryGetComponentInChildren(out IHealth health))
            return;

        health.Value += ValueChange;
    }
}
