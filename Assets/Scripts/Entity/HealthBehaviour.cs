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

                if (newHealth == _maxHealth)        // If health has reached full capacity.
                    _onHealthFullEvent.Invoke();

                if (newHealth <= 0)                 // Die if health has been depleted.
                    _onDeathEvent.Invoke();

                if (newHealth > 0 && _health <= 0)  // Resurrect if health is restored after being zero.
                    _onResurrectEvent.Invoke();
            }

            _health = newHealth;
        }
    }

    [SerializeField, MinValue(0)]
#if UNITY_EDITOR
    [OnValueChanged(nameof(UpdateHealth))]
#endif
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
    private UnityEvent _onDeathEvent;
    [SerializeField]
    private UnityEvent _onResurrectEvent;

    public void SetHealth(int value) => Value = value;

    public void SetMaxHealth(int value) => MaxValue = value;

    public void SetHealthFull()
        => Value = _maxHealth;

    public void SetHealthEmpty()
        => Value = 0;

#if UNITY_EDITOR
    private Color GetHealthBarColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / MaxValue, 2));
    }

    private void UpdateHealth()
    {
        Value = _health;
    }
#endif
}
