using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

[HideMonoScript]
public class Effector : MonoBehaviour, IEffector
{
    public GameObject GameObject => gameObject;
    public IEnumerable<IEffectAsync> Effects => _effects;

    private readonly List<IEffectAsync> _effects = new();
    private readonly List<CancellationTokenSource> _cancellationTokenSources = new();

    public bool AddEffect(IEffectBase effect, GameObject sender)
    {
        if (effect is IEffect effectSingle)
        {
            effectSingle.Effect(this, _effects, sender);
        }
        else if (effect is IEffectAsync effectAsync)
        {
            // Don't add if it is not valid.
            if (!effectAsync.IsValid(this, _effects, sender))
                return false;

            CancellationTokenSource cancellationTokenSource = new();

            _effects.Add(effectAsync);
            _cancellationTokenSources.Add(cancellationTokenSource);


            effectAsync.EffectAsync(this, _effects.Where(x => x != effect), cancellationTokenSource.Token, sender)
                .ContinueWith(() =>
                    {
                        int index = _effects.IndexOf(effectAsync);

                        if (index > -1)
                        {
                            _effects.RemoveAt(index);
                            _cancellationTokenSources.RemoveAt(index);

                            CTSUtility.Clear(ref cancellationTokenSource);
                        }
                    });
        }

        return true;
    }

    public bool AddEffects(IEnumerable<IEffectBase> effects, GameObject sender)
    {
        bool isAllValid = true;

        foreach (var effect in effects)
        {
            if (!AddEffect(effect, sender))
                isAllValid = false;
        }

        return isAllValid;
    }

    public bool RemoveEffect(IEffectAsync effect)
    {
        int index = _effects.IndexOf(effect);

        if (index < 0)
            return false;

        var cancellationTokenSource = _cancellationTokenSources[index];
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();

        _effects.RemoveAt(index);
        _cancellationTokenSources.RemoveAt(index);

        return true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < _cancellationTokenSources.Count; i++)
        {
            var cancellationTokenSource = _cancellationTokenSources[i];
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        _effects.Clear();
        _cancellationTokenSources.Clear();
    }
}
