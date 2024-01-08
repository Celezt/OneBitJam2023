using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class DurationEffect : IEffectAsync
{
    public string Tag;
    public float ValueChange = -1;
    public float Duration = 5;
    [Indent]
    public int Cycles = 5;

    public async UniTask Effect(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken)
    {
        float initialTime = Time.time;

        while (Time.time - initialTime < Duration && !cancellationToken.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(Duration / Cycles, cancellationToken: cancellationToken);

            effector.Health += ValueChange;
        }
    }

    public bool IsValid(IEffector effector, IEnumerable<IEffectAsync> effects)
        => !effects.Any(x => x is DurationEffect effect && effect.Tag == Tag);
}
