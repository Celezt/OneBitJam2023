using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class HealthDurationChangeEffect : IEffectAsync, IEffectTag, IEffectValid, IHealthChange
{
    string IEffectTag.Tag => Tag;

    public string Tag;
    public float ValueChange = -1;
    [Unit(Units.Second)]
    public float Duration = 5;
    [Indent]
    public int Cycles = 5;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {

    }

    public async UniTask UpdateAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender)
    {
        float initialTime = Time.time;

        if (!effector.GameObject.TryGetComponent(out IHealth health))
            return;

        while (Time.time - initialTime < Duration && !cancellationToken.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(Duration / Cycles, cancellationToken: cancellationToken);

            health.Value += ValueChange;
        }
    }

    public bool IsValid(IEffector effector, IEffect effect, GameObject sender)
    {
        if (effector.Effects.Any(x => x is IHealthChange healthChange && healthChange.Tag == Tag))
            return false;

        return true;
    }
}
