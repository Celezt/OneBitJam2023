using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[HideMonoScript]
public class PowerSlider : MonoBehaviour
{
    [SerializeField]
    private Slider _truePowerSlider;
    [SerializeField]
    private Slider _powerSlider;
    [SerializeField, Space(4), MinValue(0)]
    private float _minApproximation = 0.1f;

    [SerializeField, Space(8), MinValue(0)]
    private int _playerIndex;

    private IPower _power;

    public void UpdateSlider()
    {
        if (_power == null)
            return;

        float truePowerInterval = 
            Mathf.Max(_power.MaxValue <= float.Epsilon ? 0 : _minApproximation, _power.MaxValue / _power.MaxLimitValue);
        float powerInterval = 
            Mathf.Max(_power.Value <= float.Epsilon ? 0 : _minApproximation, Mathf.Min(_power.Value / _power.MaxLimitValue, truePowerInterval));

        _truePowerSlider.value = truePowerInterval;
        _powerSlider.value = powerInterval;
    }

    private void OnEnable()
    {
        var playerInput = PlayerInput.GetPlayerByIndex(_playerIndex);

        if (playerInput)
            _power = playerInput.GetComponentInParent<IPower>();

        if (_power != null)
        {
            UpdateSlider();
            _power.OnMaxLimitValueChangedCallback += OnValueChanged;
            _power.OnMaxValueChangedCallback += OnValueChanged;
            _power.OnValueChangedCallback += OnValueChanged;
        }
    }

    private void OnDisable()
    {
        if (_power != null)
        {
            _power.OnMaxLimitValueChangedCallback -= OnValueChanged;
            _power.OnMaxValueChangedCallback -= OnValueChanged;
            _power.OnValueChangedCallback -= OnValueChanged;
        }

        _power = null;
    }

    private void OnValueChanged(float newValue, float oldValue) 
        => UpdateSlider();
}
