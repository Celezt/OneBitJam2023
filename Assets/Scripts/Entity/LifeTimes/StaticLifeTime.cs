using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StaticLifeTime : ILifeTime
{
    [SuffixLabel("sec", overlay: true), MinValue(0)]
    public float Duration = 2;

    public async UniTask UpdateAsync(CancellationToken cancellationToken, IEntity entity)
    {
        await UniTask.WaitForSeconds(Duration, cancellationToken: cancellationToken);

        entity.Destroy();
    }
}
