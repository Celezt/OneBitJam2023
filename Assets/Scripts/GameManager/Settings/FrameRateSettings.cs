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
    
    public void GameStart(IEnumerable<ISettings> settings)
    {
        OnSetFrameRate();
        OnSetVSync();
    }

    public void GameExit(IEnumerable<ISettings> settings)
    {

    }

    private void OnSetFrameRate()
    {
        if (_fixedFrameRate)
            Application.targetFrameRate = _frameRate;
        else
            Application.targetFrameRate = -1;
    }

    private void OnSetVSync()
    {
        if (_vSync)
        {
            float vSyncFactor = (float)Screen.currentResolution.refreshRateRatio.value / 60.0f;
            QualitySettings.vSyncCount = Mathf.Clamp(Mathf.RoundToInt(vSyncFactor), 1, 4);
        }
        else
            QualitySettings.vSyncCount = 0;
    }
}
