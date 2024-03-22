using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class MinAttribute : Attribute
{
    public float MinValue { get; }

    public MinAttribute(float minValue)
    {
        MinValue = minValue;
    }
}
