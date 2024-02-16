using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[HideMonoScript]
public class PercentageText : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI _textMeshPro;
    [SerializeField]
    private string _beforeText;
    [SerializeField]
    private string _afterText = "%";

    private char[] _buffer = new char[128];

    public void SetPercentage(float value)
    {
        value = Mathf.Clamp01(value);

        int beforeLength = _beforeText?.Length ?? 0;
        int afterLength = _afterText?.Length ?? 0;

        if (!string.IsNullOrEmpty(_beforeText))
            _beforeText.CopyTo(0, _buffer, 0, beforeLength);

        int length = ConvertExtensions.ToCharArray((uint)(Mathf.RoundToInt(value * 100)), _buffer, beforeLength);

        if (!string.IsNullOrEmpty(_afterText))
            _afterText.CopyTo(0, _buffer, length + beforeLength, afterLength);

        _textMeshPro.SetCharArray(_buffer, 0, length + beforeLength + afterLength);
    }
}
