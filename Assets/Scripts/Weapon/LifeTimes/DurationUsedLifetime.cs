using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class DurationUsedLifetime : ILifetime
{
    public AnimationCurve LifeTimeByDurationUsedCurve = AnimationCurve.Linear(0, 1, 1, 0);

    public UnityEvent<float> OnLifeTimeByDurationUsed;

    public async UniTask UpdateAsync(CancellationToken cancellationToken, IEntity entity, Weapon.CallbackContext context)
    {
        float duration = LifeTimeByDurationUsedCurve.Evaluate(context.DurationUsed);

        OnLifeTimeByDurationUsed.Invoke(duration);

        await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);

        entity.Destroy();
    }
}
