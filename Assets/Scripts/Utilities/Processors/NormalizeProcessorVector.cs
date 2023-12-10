using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampProcessorVector : IProcessor<Vector2>
{
    [SerializeField]
    private float _minValue;
    [SerializeField] 
    private float _maxValue;

    public Vector2 Process(Vector2 value)
        => new Vector2(Mathf.Clamp(value.x, _minValue, _maxValue), Mathf.Clamp(value.y, _minValue, _maxValue));
}
