using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class ConditionBeforeDestroyHit : IHitAsync
{
    public Condition Condition;
    [Indent]
    public bool Inverse;

    [Space(8)]
    public UnityEvent OnBeforeDestroyEvent;
    public UnityEvent OnAfterDestroyEvent;

    public void Initialize(IEntity entity)
    {
        
    }

    public async UniTaskVoid UpdateAsync(CancellationToken cancellationToken, IEntity entity)
    {
        OnBeforeDestroyEvent.Invoke();

        await UniTask.WaitUntil(() => Inverse ? !Condition.OnCondition() : Condition.OnCondition(), cancellationToken: cancellationToken);

        OnAfterDestroyEvent.Invoke();

        entity.Destroy();
    }
}
