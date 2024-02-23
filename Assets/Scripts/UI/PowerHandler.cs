using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[HideMonoScript]
public class PowerHandler : MonoBehaviour
{
    public IPower Power
    {
        get
        {
            if (_power == null && _receiver != null)
                _power = _receiver.Receive.GetComponentInParent<IPower>();

            return _power;
        }
    }
    public float PowerPercentage => Power.Value / Power.MaxLimitValue;
    public float TruePowerDifference => Power.MaxValue / Power.MaxLimitValue - PowerPercentage;

    [SerializeField]
    private Receiver _receiver;

    [SerializeField, Space(8)]
    private UnityEvent<float> _onPowerPercentageChangedEvent;
    [SerializeField]
    private UnityEvent<float> _onTruePowerDifferenceChangedEvent;

    private IPower _power;

    private void OnEnable()
    {
        if (Power != null)
        {
            _onPowerPercentageChangedEvent.Invoke(PowerPercentage);
            _onTruePowerDifferenceChangedEvent.Invoke(TruePowerDifference);

            Power.OnMaxLimitValueChangedCallback += OnValueChanged;
            Power.OnMaxValueChangedCallback += OnValueChanged;
            Power.OnValueChangedCallback += OnValueChanged;
        }
    }

    private void OnDisable()
    {
        if (Power != null)
        {
            Power.OnMaxLimitValueChangedCallback -= OnValueChanged;
            Power.OnMaxValueChangedCallback -= OnValueChanged;
            Power.OnValueChangedCallback -= OnValueChanged;
        }

        _power = null;
    }

    private void OnValueChanged(float oldValue, float newValue)
    {
        _onPowerPercentageChangedEvent.Invoke(PowerPercentage);
        _onTruePowerDifferenceChangedEvent.Invoke(TruePowerDifference);
    }
}
