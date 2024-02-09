using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
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
    public IEnumerable<IEffectorProperty> Properties => _properties;

    public event Action<IEffector, IEffect, GameObject> OnEffectAddedCallback = delegate { };
    public event Action<IEffector, IEffect> OnEffectRemovedCallback = delegate { };

    [SerializeReference]
    private List<IEffectorProperty> _properties = new();

    private readonly List<IEffectAsync> _effects = new();
    private readonly List<CancellationTokenSource> _cancellationTokenSources = new();

    public bool AddEffect(IEffect effect, GameObject sender)
    {
        effect.Initialize(this, _effects, sender);

        // Don't add if it does not pass all properties with valid check.
        if (!_properties.OfType<IEffectValid>().All(x => x.IsValid(this, effect, sender)))
            return false;

        // Don't add if it is not valid.
        if (effect is IEffectValid validEffect && !validEffect.IsValid(this, effect, sender))
            return false;

        if (effect is IEffectAsync effectAsync)
        {
            CancellationTokenSource cancellationTokenSource = new();

            _effects.Add(effectAsync);
            _cancellationTokenSources.Add(cancellationTokenSource);

            effectAsync.UpdateAsync(this, _effects.Where(x => x != effect), cancellationTokenSource.Token, sender)
                .ContinueWith(() =>
                    {
                        int index = _effects.IndexOf(effectAsync);

                        if (index > -1)
                        {
                            _effects.RemoveAt(index);
                            _cancellationTokenSources[index].Dispose();
                            _cancellationTokenSources.RemoveAt(index);
                        }
                    });
        }

        OnEffectAddedCallback(this, effect, sender);

        return true;
    }

    public bool AddEffects(IEnumerable<IEffect> effects, GameObject sender)
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

        OnEffectRemovedCallback(this, effect);

        return true;
    }

    public void AddProperty(IEffectorProperty property)
    {
        property.OnEnable(this);
        _properties.Add(property);
    }

    public void RemoveProperty(IEffectorProperty property)
    {
        property.OnDisable(this);
        _properties.Remove(property);
    }

    private void Start()
    {
        foreach (var property in _properties)
            property.OnEnable(this);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _cancellationTokenSources.Count; i++)
        {
            var cancellationTokenSource = _cancellationTokenSources[i];
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
