using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IEffector
{
    public float MaxValue { get; set; }
    public float Value { get; set; }

    public IEnumerable<IEffectAsync> Effects { get; }

    public bool AddEffect(IEffect effect);
    public bool AddEffects(IEnumerable<IEffect> effects);
    public bool RemoveEffect(IEffectAsync effect);
}
