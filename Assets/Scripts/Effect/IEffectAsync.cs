using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IEffectAsync : IEffect
{
    public UniTask Effect(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken);

    public bool IsValid(IEffector effector, IEnumerable<IEffectAsync> effects);
}
