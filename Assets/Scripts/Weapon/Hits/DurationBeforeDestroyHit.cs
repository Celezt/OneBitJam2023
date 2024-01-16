using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class DurationBeforeDestroyHit : IHitAsync
{
    [SuffixLabel("sec", overlay: true), MinValue(0)]
    public float Duration = 1;

    [Space(8)]
    public UnityEvent OnBeforeDestroyEvent;
    public UnityEvent OnAfterDestroyEvent;

    public void Initialize(IEntity entity)
    {
        
    }

    public async UniTaskVoid UpdateAsync(CancellationToken cancellationToken, IEntity entity)
    {
        OnBeforeDestroyEvent.Invoke();

        await UniTask.WaitForSeconds(Duration, cancellationToken: cancellationToken);

        OnAfterDestroyEvent.Invoke();

        entity.Destroy();
    }
}
