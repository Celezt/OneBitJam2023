using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizeProcessorVector : IProcessor<Vector2>
{
    public Vector2 Process(Vector2 value)
        => value.normalized;
}
