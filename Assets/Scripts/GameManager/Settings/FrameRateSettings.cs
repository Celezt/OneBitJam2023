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
    
    public void GameStart(IEnumerable<ISettings> settings)
    {
        OnSetFrameRate();
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
}
