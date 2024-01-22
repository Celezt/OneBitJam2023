using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class HealthDurationChangeEffect : IEffectAsync, IElement
{
    string IElement.Element => Element;

    public string Element;
    public float ValueChange = -1;
    public float Duration = 5;
    [Indent]
    public int Cycles = 5;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {

    }

    public async UniTask UpdateAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender)
    {
        float initialTime = Time.time;

        if (!effector.GameObject.TryGetComponentInChildren(out IHealth health))
            return;

        while (Time.time - initialTime < Duration && !cancellationToken.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(Duration / Cycles, cancellationToken: cancellationToken);

            health.Value += ValueChange;
        }
    }

    public bool IsValid(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        if (effector.Properties.Any(x => x is IElementalImmunity immunity && immunity.Element == Element))
            return false;

        if (effects.Any(x => x is IElement effect && effect.Element == Element))
            return false;

        return true;
    }
}
