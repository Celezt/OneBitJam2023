using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[HideMonoScript]
public class PowerSlider : MonoBehaviour
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

    [SerializeField]
    private Receiver _receiver;
    [SerializeField, DisableInPlayMode]
    private Slider _truePowerSlider;
    [SerializeField, DisableInPlayMode]
    private Slider _powerSlider;
    [SerializeField, Space(4), MinValue(0)]
    private float _minApproximation = 0.1f;

    [SerializeField]
    private bool _useLerp = true;
    [SerializeField, ShowIf(nameof(_useLerp)), Indent]
    private float _speed = 4;

    [SerializeField, Space(8)]
    private UnityEvent<float> _onPowerIncreasedEvent;
    [SerializeField]
    private UnityEvent<float> _onPowerDecreasedEvent;

    private float _accumulatedTime;
    private IPower _power;
    private CancellationTokenSource _cancellationTokenSource;

    public void UpdateSliderInstant()
    {
        if (Power == null || !_truePowerSlider || !_powerSlider)
            return;

        (_truePowerSlider.value, _powerSlider.value) = GetIntervals();
    }

    public void UpdateSlider()
    {
        if (Power == null || !_truePowerSlider || !_powerSlider)
            return;

        if (_useLerp)
        {
            _accumulatedTime = 0;
            if (_cancellationTokenSource == null)
            {
                CTSUtility.Reset(ref _cancellationTokenSource);
                UpdateSliderAsync(_cancellationTokenSource.Token)
                    .ContinueWith(() => CTSUtility.Clear(ref _cancellationTokenSource));
            }
        }
        else
            UpdateSliderInstant();

        async UniTask UpdateSliderAsync(CancellationToken cancellationToken)
        {
            float previousTruePowerInterval = 0;
            float previousPowerInterval = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                (float truePowerInterval, float powerInterval) = GetIntervals();

                if (Mathf.Approximately(_truePowerSlider.value, truePowerInterval) &&
                    Mathf.Approximately(_powerSlider.value, powerInterval))
                    break;

                _truePowerSlider.value = Mathf.Lerp(_truePowerSlider.value, truePowerInterval, _accumulatedTime * _speed);
                _powerSlider.value = Mathf.Lerp(_powerSlider.value, powerInterval, _accumulatedTime * _speed);

                _accumulatedTime += Time.deltaTime;

                previousTruePowerInterval = truePowerInterval;
                previousPowerInterval = powerInterval;

                await UniTask.Yield(cancellationToken);
            }
        }
    }

    private void OnEnable()
    {
        if (Power != null)
        {
            UpdateSliderInstant();
            Power.OnMaxLimitValueChangedCallback += OnMaxPowerChanged;
            Power.OnMaxValueChangedCallback += OnTrueValueChanged;
            Power.OnValueChangedCallback += OnPowerChanged;
        }
    }

    private void OnDisable()
    {
        if (Power != null)
        {
            Power.OnMaxLimitValueChangedCallback -= OnMaxPowerChanged;
            Power.OnMaxValueChangedCallback -= OnTrueValueChanged;
            Power.OnValueChangedCallback -= OnPowerChanged;
        }

        _power = null;

        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private void OnMaxPowerChanged(float newValue, float oldValue)
    {
        UpdateSlider();
    }

    private void OnTrueValueChanged(float newValue, float oldValue)
    {
        UpdateSlider();
    }

    private void OnPowerChanged(float newValue, float oldValue)
    {
        UpdateSlider();

        if (newValue > oldValue)
            _onPowerIncreasedEvent.Invoke(Power.Value / Power.MaxLimitValue);
        else if (newValue < oldValue)
            _onPowerDecreasedEvent.Invoke(Power.Value / Power.MaxLimitValue);
    }

    private (float TruePowerInterval, float PowerInterval) GetIntervals()
    {
        float maxPower = Power.MaxLimitValue;
        float truePower = Power.MaxValue;
        float power = Power.Value;

        float truePowerInterval =
            Mathf.Max(truePower <= float.Epsilon ? 0 : _minApproximation, truePower / maxPower);
        float powerInterval =
            Mathf.Max(power <= float.Epsilon ? 0 : _minApproximation, Mathf.Min(power / maxPower, truePowerInterval));

        return (truePowerInterval, powerInterval);
    }
}
