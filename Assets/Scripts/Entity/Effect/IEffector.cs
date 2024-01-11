using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IEffector
{
    public GameObject GameObject { get; }
    public IEnumerable<IEffectAsync> Effects { get; }

    public bool AddEffect(IEffectBase effect, GameObject sender);
    public bool AddEffects(IEnumerable<IEffectBase> effects, GameObject sender);
    public bool RemoveEffect(IEffectAsync effect);
}
