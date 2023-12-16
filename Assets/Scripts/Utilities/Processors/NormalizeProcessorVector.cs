using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizeProcessorVector : IProcessor<Vector2>
{
    [SerializeField]
    private Mode _mode = Mode.Analog;
    [SerializeField, HideIf(nameof(_mode), Mode.Analog)]
    private float _precision = 0.1f;

    private enum Mode
    {
        Analog,
        Digital,
        DigitalNormalized,
    }

    public Vector2 Process(Vector2 value) => _mode switch
    {
        Mode.Analog => value.normalized,
        Mode.Digital => Digital(value),
        Mode.DigitalNormalized => Digital(value).normalized,
        _ => throw new System.NotImplementedException(),
    };

    private Vector2 Digital(Vector2 vector)
    {
        float x = AlmostEquals(vector.x, 0f, _precision) ? 0f : vector.x > 0f ? 1f : -1f;
        float y = AlmostEquals(vector.y, 0f, _precision) ? 0f : vector.y > 0f ? 1f : -1f;

        return new Vector2(x, y);
    }

    private static bool AlmostEquals(float valueFirst, float valueSecond, float precision)
        => Mathf.Abs(valueFirst - valueSecond) <= precision;
}
