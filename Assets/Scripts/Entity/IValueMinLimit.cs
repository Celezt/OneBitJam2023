using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueMinLimit
{
    public float MinLimitValue { get; set; }
    public event Action<float, float> OnMinLimitValueChangedCallback;
}
