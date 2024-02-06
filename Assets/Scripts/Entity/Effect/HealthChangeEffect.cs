using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthChangeEffect : IEffect, IHealthChange
{
    string IEffectTag.Tag => Tag;

    public string Tag;
    public float ValueChange = -10;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        if (!effector.GameObject.TryGetComponent(out IHealth health))
            return;

        if (health.Value > 0)
            health.Value += ValueChange;
    }
}
