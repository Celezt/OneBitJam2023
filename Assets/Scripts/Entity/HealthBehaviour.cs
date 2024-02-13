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
public class HealthBehaviour : MonoBehaviour, IHealth
{
    public float MaxValue
    {
        get => _maxHealth;
        set
        {
            float newMaxHealth = Mathf.Max(value, 0);
            float oldMaxHealth = _maxHealth;
            _maxHealth = newMaxHealth;

#if UNITY_EDITOR
            _oldMaxHealth = _maxHealth;
#endif

            if (newMaxHealth != oldMaxHealth) // If any change has been made. 
            {
                _onMaxHealthChangedEvent.Invoke(newMaxHealth, oldMaxHealth);
                OnMaxValueChangedCallback(newMaxHealth, oldMaxHealth);
            }

            Value = _health;   // Update if the new max health is less than the current health.
        }
    }
    public float Value
    {
        get => _health;
        set
        {
            float newHealth = Mathf.Clamp(value, 0, MaxValue);
            float oldHealth = _health;
            _health = newHealth;

#if UNITY_EDITOR
            _oldHealth = _health;
#endif

            if (newHealth != oldHealth)   // If any change has been made.
            {
                _onHealthChangedEvent.Invoke(newHealth, oldHealth);
                OnValueChangedCallback(newHealth, oldHealth);

                if (newHealth == _maxHealth)        // If health has reached full capacity.
                    _onHealthFullEvent.Invoke();
                if (newHealth <= 0)  // Die if health has been depleted.
                    _onDeathEvent.Invoke();

                if (newHealth > 0 && oldHealth <= 0)  // Resurrect if health is restored after being zero.
                    _onResurrectEvent.Invoke();
            }
        }
    }

    public event Action<float, float> OnValueChangedCallback = delegate { };
    public event Action<float, float> OnMaxValueChangedCallback = delegate { };

    [SerializeField, MinValue(0)]
#if UNITY_EDITOR
    [OnValueChanged(nameof(UpdateHealth))]
#endif
    private float _maxHealth = 100;
#if UNITY_EDITOR
    [ProgressBar(0, nameof(_maxHealth), ColorGetter = nameof(GetHealthBarColor))]
    [OnValueChanged(nameof(UpdateHealth))]
#endif
    [SerializeField, Indent]
    private float _health = 100;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<float, float> _onHealthChangedEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<float, float> _onMaxHealthChangedEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent _onHealthFullEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent _onDeathEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent _onResurrectEvent;

    public void SetHealth(int value) => Value = value;

    public void SetMaxHealth(int value) => MaxValue = value;

    public void SetHealthFull()
        => Value = _maxHealth;

    public void SetHealthEmpty()
        => Value = 0;

#if UNITY_EDITOR
    private float _oldHealth;
    private float _oldMaxHealth;

    private Color GetHealthBarColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / MaxValue, 2));
    }

    private void UpdateHealth()
    {
        float newMaxHealth = Mathf.Max(0, _maxHealth);
        float oldMaxHealth = _oldMaxHealth;

        _oldMaxHealth = _maxHealth;
        _maxHealth = newMaxHealth;

        float newHealth = Mathf.Clamp(_health, 0, MaxValue);
        float oldHealth = _oldHealth;

        _oldHealth = _health;
        _health = newHealth;

        if (newMaxHealth !=  oldMaxHealth)
        {
            _onMaxHealthChangedEvent.Invoke(newMaxHealth, oldMaxHealth);
            OnMaxValueChangedCallback(newMaxHealth, oldMaxHealth);
        }

        if (newHealth != oldHealth)   // If any change has been made.
        {
            _onHealthChangedEvent.Invoke(newHealth, oldHealth);
            OnValueChangedCallback(newHealth, oldHealth);

            if (newHealth == _maxHealth)        // If health has reached full capacity.
                _onHealthFullEvent.Invoke();
            if (newHealth <= 0)  // Die if health has been depleted.
                _onDeathEvent.Invoke();

            if (newHealth > 0 && oldHealth <= 0)  // Resurrect if health is restored after being zero.
                _onResurrectEvent.Invoke();
        }
    }
#endif
}
