using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValue
{
    public float Value { get; set; }
    public event Action<float, float> OnValueChangedCallback;
}
