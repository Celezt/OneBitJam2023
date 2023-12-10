using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpProcessorVector : IProcessor<Vector2>
{
    [SerializeField]
    private float _lerpSpeed = 1.0f;

    private Vector2 _lerp;

    public Vector2 Process(Vector2 value)
    {
        _lerp = Vector3.Lerp(_lerp, value, Time.deltaTime * _lerpSpeed);

        return _lerp;
    }
}
