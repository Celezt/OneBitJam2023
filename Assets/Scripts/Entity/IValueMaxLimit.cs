using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueMaxLimit
{
    public float MaxLimitValue { get; set; }
    public event Action<float, float> OnMaxLimitValueChangedCallback;
}
