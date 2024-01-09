using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IEffectAsync : IEffectBase
{
    public UniTask EffectAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender);

    public bool IsValid(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender);
}
