using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[HideMonoScript]
public class VolumeBinder : MonoBehaviour
{
    [SerializeField] 
    private AudioMixer _audioMixer;

    [SerializeField] 
    private string _parameterName;

    [SerializeField, Space(8)]
    private UltEvent<float> _onVolumeChangedEvent;

    public void SetLevel(float linearValue)
    {
        linearValue = Mathf.Max(linearValue, 0.0001f);  // Can never reach 0.

        if (string.IsNullOrWhiteSpace(_parameterName))
            return;

        _audioMixer.SetFloat(_parameterName, Mathf.Log10(linearValue) * 20.0f);
    }

    private async void OnEnable()
    {
        await UniTask.Yield();

        if (_audioMixer.GetFloat(_parameterName, out float outLevel))
        {
            float linearValue = Mathf.Pow(10, outLevel / 20.0f);

            _onVolumeChangedEvent.Invoke(linearValue);
        }
    }
}
