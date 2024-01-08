using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class HealthBehaviour : MonoBehaviour, IEffector
{
    public float MaxValue
    {
        get => _maxHealth;
        set
        {
            float newMaxHealth = Mathf.Max(value, 0);

            if (newMaxHealth != _maxHealth) // If any change has been made. 
                _onMaxHealthChangedEvent.Invoke(newMaxHealth, _maxHealth);

            _maxHealth = newMaxHealth;

            Value = _health;   // Update if the new max health is less than the current health.
        }
    } 

    public float Value
    {
        get => _health;
        set
        {
            float newHealth = Mathf.Clamp(value, 0, MaxValue);

            if (newHealth != _health)   // If any change has been made.
            {
                _onHealthChangedEvent.Invoke(newHealth, _health);

                if (newHealth == _maxHealth)    // If health has reached full capacity.
                    _onHealthFullEvent.Invoke();

                if (newHealth <= 0)             // If health has been depleted.
                    _onHealthEmptyEvent.Invoke();
            }

            _health = newHealth;
        }
    }

    public IEnumerable<IEffectAsync> Effects => _effects;

    [SerializeField, MinValue(0)]
    private float _maxHealth = 100;
    [SerializeField, Indent]
#if UNITY_EDITOR
    [ProgressBar(0, nameof(_maxHealth), ColorGetter = nameof(GetHealthBarColor))]
#endif
    private float _health = 100;

    [SerializeField]
    private UnityEvent<float, float> _onHealthChangedEvent;
    [SerializeField]
    private UnityEvent<float, float> _onMaxHealthChangedEvent;
    [SerializeField]
    private UnityEvent _onHealthFullEvent;
    [SerializeField]
    private UnityEvent _onHealthEmptyEvent;

    private readonly List<IEffectAsync> _effects = new();
    private readonly List<CancellationTokenSource> _cancellationTokenSources = new();

    public void SetHealth(int value) => Value = value;

    public void SetMaxHealth(int value) => MaxValue = value;

    public bool AddEffect(IEffect effect)
    {
        if (effect is IEffectSingle effectSingle)
        {
            effectSingle.Effect(this, _effects);
        }
        else if (effect is IEffectAsync effectAsync)
        {
            // Don't add if it is not valid.
            if (!effectAsync.IsValid(this, _effects))
                return false;

            CancellationTokenSource cancellationTokenSource = new();

            _effects.Add(effectAsync);
            _cancellationTokenSources.Add(cancellationTokenSource);
            

            effectAsync.EffectAsync(this, _effects.Where(x => x != effect), cancellationTokenSource.Token).ContinueWith(() =>
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

    public bool AddEffects(IEnumerable<IEffect> effects)
    {
        bool isAllValid = true;

        foreach (var effect in effects)
        {
            if (!AddEffect(effect))
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

#if UNITY_EDITOR
    private Color GetHealthBarColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / MaxValue, 2));
    }
#endif
}
