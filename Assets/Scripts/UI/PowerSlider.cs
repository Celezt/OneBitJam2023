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
    public int PlayerIndex
    {
        get => _playerIndex;
        set => SetPlayerIndex(value);
    }

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

    [SerializeField, Space(8), MinValue(0), OnValueChanged(nameof(SetPlayerIndex))]
    private int _playerIndex;

    [SerializeField, Space(8)]
    private UnityEvent _onPowerIncreasedEvent;
    [SerializeField]
    private UnityEvent _onPowerDecreasedEvent;

    private float _accumulatedTime;
    private IPower _power;
    private CancellationTokenSource _cancellationTokenSource;

    public void SetPlayerIndex(int playerIndex)
    {
        _playerIndex = playerIndex;
        var playerInput = PlayerInput.GetPlayerByIndex(_playerIndex);

        if (playerInput)
            _power = playerInput.GetComponentInParent<IPower>();
    }

    public void UpdateSliderInstant()
    {
        if (_power == null || !_truePowerSlider || !_powerSlider)
            return;

        (float truePowerInterval, float powerInterval) = GetIntervals();

        _truePowerSlider.value = truePowerInterval;
        _powerSlider.value = powerInterval;
    }

    public void UpdateSlider()
    {
        if (_power == null || !_truePowerSlider || !_powerSlider)
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
        SetPlayerIndex(_playerIndex);

        if (_power != null)
        {
            UpdateSliderInstant();
            _power.OnMaxLimitValueChangedCallback += OnValueChanged;
            _power.OnMaxValueChangedCallback += OnValueChanged;
            _power.OnValueChangedCallback += OnPowerChanged;
        }
    }

    private void OnDisable()
    {
        if (_power != null)
        {
            _power.OnMaxLimitValueChangedCallback -= OnValueChanged;
            _power.OnMaxValueChangedCallback -= OnValueChanged;
            _power.OnValueChangedCallback -= OnPowerChanged;
        }

        _power = null;

        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private void OnValueChanged(float newValue, float oldValue)
        => UpdateSlider();

    private void OnPowerChanged(float newValue, float oldValue)
    {
        UpdateSlider();

        if (newValue > oldValue)
            _onPowerIncreasedEvent.Invoke();
        else if (newValue < oldValue)
            _onPowerDecreasedEvent.Invoke();
    }

    private (float TruePowerInterval, float PowerInterval) GetIntervals()
    {
        float maxPower = _power.MaxLimitValue;
        float truePower = _power.MaxValue;
        float power = _power.Value;

        float truePowerInterval =
            Mathf.Max(truePower <= float.Epsilon ? 0 : _minApproximation, truePower / maxPower);
        float powerInterval =
            Mathf.Max(power <= float.Epsilon ? 0 : _minApproximation, Mathf.Min(power / maxPower, truePowerInterval));

        return (truePowerInterval, powerInterval);
    }
}
