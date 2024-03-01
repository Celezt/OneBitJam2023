using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[HideMonoScript]
public class SliderClamp : MonoBehaviour
{
    public UltEvent<float> OnValueChanged => _onValueChangedEvent;

    [SerializeField]
    private Slider _slider;
    [SerializeField, MinMaxSlider(
        "@_slider == null ? 0f : _slider.minValue",
        "@_slider == null ? 1f : _slider.maxValue", showFields: true), Indent]
    private Vector2 _clamp = new Vector2(float.MinValue, float.MinValue);

    [SerializeField, Space(8)]
    private UltEvent<float> _onValueChangedEvent;

    public void SetValue(float value)
    {
        if (!_slider)
            return;

        _slider.value = Mathf.Clamp(value, _clamp.x, _clamp.y);

        _onValueChangedEvent.Invoke(value);
    }

    private void OnValidate()
    {
        if (_clamp == new Vector2(float.MinValue, float.MinValue) && _slider)
            _clamp = new Vector2(_slider.minValue, _slider.maxValue);
    }

    private void OnEnable()
    {
        if (!_slider)
            return;

        _slider.onValueChanged.AddListener(SetSliderValue);
    }

    private void OnDisable()
    {
        if (!_slider)
            return;

        _slider.onValueChanged.RemoveListener(SetSliderValue);
    }

    private void SetSliderValue(float value)
    {
        if (!_slider)
            return;

        _slider.value = Mathf.Clamp(value, _clamp.x, _clamp.y);

        float actualValue = Remap(_clamp.x, _clamp.y, _slider.minValue, _slider.maxValue, value);

        _onValueChangedEvent.Invoke(actualValue);
    }

    private float Remap(float fromMin, float fromMax, float toMin, float toMax, float value)
    {
        if (value < fromMin)
            return toMin;
        else if (value > fromMax)
            return toMax;

        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
