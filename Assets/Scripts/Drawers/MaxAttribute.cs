using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class MaxAttribute : Attribute
{
    public float MaxValue { get; }

    public MaxAttribute(float maxValue)
    {
        MaxValue = maxValue;
    }
}
