using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

[HideMonoScript]
public class PowerBehaviour : MonoBehaviour, IPower
{
    public float MaxLimitValue
    {
        get => _maxPower;
        set
        {
            float newMaxPower = Mathf.Max(value, 0);
            float oldMaxPower = _maxPower;
            _maxPower = newMaxPower;

#if UNITY_EDITOR
            _oldMaxPower = _maxPower;
#endif

            if (newMaxPower != oldMaxPower)
            {
                _onMaxPowerChangedEvent.Invoke(newMaxPower, oldMaxPower);
                OnMaxLimitValueChangedCallback(newMaxPower, oldMaxPower);
            }
        }
    }

    public float MaxValue
    {
        get => _truePower;
        set
        {
            float newTruePower = Mathf.Clamp(value, 0, MaxLimitValue);
            float oldTruePower = _truePower;
            _truePower = newTruePower;

#if UNITY_EDITOR
            _oldTruePower = _truePower;
#endif

            if (newTruePower != oldTruePower) // If any change has been made. 
            {
                _onTruePowerChangedEvent.Invoke(newTruePower, oldTruePower);
                OnMaxValueChangedCallback(newTruePower, oldTruePower);
            }


            Value = _power;   // Update if the new true power is less than the current power.
        }
    }
    public float Value
    {
        get => _power;
        set
        {
            float newPower = Mathf.Clamp(value, 0, MaxValue);
            float oldPower = _power;
            _power = newPower;

#if UNITY_EDITOR
            _oldPower = _power;
#endif

            if (newPower != oldPower)   // If any change has been made.
            {
                _onPowerChangedEvent.Invoke(newPower, oldPower);
                OnValueChangedCallback(newPower, oldPower);

                if (newPower == _truePower)        // If health has reached full capacity.
                    _onPowerFullEvent.Invoke();
                if (newPower <= 0)  // Die if health has been depleted.
                    _onDeathEvent.Invoke();

                if (newPower > 0 && oldPower <= 0)  // Resurrect if health is restored after being zero.
                    _onResurrectEvent.Invoke();
            }

        }
    }

    public event Action<float, float> OnValueChangedCallback = delegate { };
    public event Action<float, float> OnMaxValueChangedCallback = delegate { };
    public event Action<float, float> OnMaxLimitValueChangedCallback = delegate { };

#if UNITY_EDITOR
    [OnValueChanged(nameof(UpdatePower))]
#endif
    [SerializeField, MinValue(0)]
    private float _maxPower = 100;
#if UNITY_EDITOR
    [ProgressBar(0, nameof(_maxPower), 0.3f, 0.3f, 1f)]
    [OnValueChanged(nameof(UpdatePower))]
#endif
    [SerializeField, Indent]
    private float _truePower = 100;
#if UNITY_EDITOR
    [ProgressBar(0, nameof(_truePower), ColorGetter = nameof(GetHealthBarColor))]
    [OnValueChanged(nameof(UpdatePower))]
#endif
    [SerializeField, Indent(2)]
    private float _power = 100;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<float, float> _onPowerChangedEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<float, float> _onTruePowerChangedEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<float, float> _onMaxPowerChangedEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent _onPowerFullEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent _onDeathEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent _onResurrectEvent;

    public void SetPower(int value) => Value = value;

    public void SetTruePower(int value) => MaxValue = value;

    public void SetPowerFull()
        => Value = _truePower;

    public void SetPowerEmpty()
        => Value = 0;

#if UNITY_EDITOR
    private float _oldMaxPower;
    private float _oldTruePower;
    private float _oldPower;

    private Color GetHealthBarColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / MaxValue, 2));
    }

    private void UpdatePower()
    {
        float newMaxPower = Mathf.Max(_maxPower, 0);
        float oldMaxPower = _maxPower;

        _oldMaxPower = _maxPower;
        _maxPower = newMaxPower;

        float newTruePower = Mathf.Clamp(_truePower, 0, _maxPower);
        float oldTruePower = _oldTruePower;

        _oldTruePower = _truePower;
        _truePower = newTruePower;

        float newPower = Mathf.Clamp(_power, 0, _truePower);
        float oldPower = _oldPower;

        _oldPower = _power;
        _power = newPower;

        if (newMaxPower !=  oldMaxPower)
        {
            _onMaxPowerChangedEvent.Invoke(newMaxPower, oldMaxPower);
            OnMaxLimitValueChangedCallback(newMaxPower, oldMaxPower);
        }

        if (newTruePower != oldTruePower) // If any change has been made. 
        {
            _onTruePowerChangedEvent.Invoke(newTruePower, oldTruePower);
            OnMaxValueChangedCallback(newTruePower, oldTruePower);
        }

        if (newPower != oldPower)   // If any change has been made.
        {
            _onPowerChangedEvent.Invoke(newPower, oldPower);
            OnValueChangedCallback(newPower, oldPower);

            if (newPower == _truePower)        // If health has reached full capacity.
                _onPowerFullEvent.Invoke();
            if (newPower <= 0)  // Die if health has been depleted.
                _onDeathEvent.Invoke();

            if (newPower > 0 && oldPower <= 0)  // Resurrect if health is restored after being zero.
                _onResurrectEvent.Invoke();
        }
    }
#endif
}
