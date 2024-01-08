using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseEffect : IEffectSingle
{
    public float ValueChange = -10;

    public void Effect(IEffector effector, IEnumerable<IEffectAsync> effects)
    {
        effector.Value += ValueChange;
    }
}
