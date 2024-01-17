using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[GlobalSettings(Order = 1000)]
public class FrameRateSettings : ISettings
{
    [SerializeField, OnValueChanged(nameof(OnSetFrameRate))]
    private bool _fixedFrameRate = false;
    [SerializeField, EnableIf(nameof(_fixedFrameRate)), Indent, OnValueChanged(nameof(OnSetFrameRate))]
    private int _frameRate = 60;
    [SerializeField, OnValueChanged(nameof(OnSetVSync)), LabelText("V-Sync")]
    private bool _vSync = false;
    [SerializeField, OnValueChanged(nameof(OnSetTimeScale))]
    private float _timeScale = 1;
    
    public void GameStart(IEnumerable<ISettings> settings)
    {
        OnSetFrameRate();
        OnSetVSync();
        OnSetTimeScale();
    }

    public void GameExit(IEnumerable<ISettings> settings)
    {

    }

    public void OnSetFrameRate()
    {
        if (_fixedFrameRate)
            Application.targetFrameRate = _frameRate;
        else
            Application.targetFrameRate = -1;
    }

    public void OnSetVSync()
    {
        if (_vSync)
        {
            float vSyncFactor = (float)Screen.currentResolution.refreshRateRatio.value / 60.0f;
            QualitySettings.vSyncCount = Mathf.Clamp(Mathf.RoundToInt(vSyncFactor), 1, 4);
        }
        else
            QualitySettings.vSyncCount = 0;
    }

    public void OnSetTimeScale()
    {
        _timeScale = Mathf.Max(_timeScale, 0);
        Time.timeScale = _timeScale;
    }
}
