using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class DurationUsedLifetime : ILifetime
{
    public AnimationCurve LifeTimeByDurationUsedCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Space(8)]
    public UnityEvent<float> OnLifeTimeByDurationUsedEvent;

    public async UniTask UpdateAsync(CancellationToken cancellationToken, IEntity entity, Firearm firearm)
    {
        float duration = LifeTimeByDurationUsedCurve.Evaluate(firearm.Handler.DurationUsed);

        OnLifeTimeByDurationUsedEvent.Invoke(duration);

        await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);

        entity.Destroy();
    }
}
