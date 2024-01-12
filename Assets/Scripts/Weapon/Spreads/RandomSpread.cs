using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpread : ISpread
{
    [SuffixLabel("degree", overlay: true)]
    public float Spread = 4;

    public Quaternion Rotation(Quaternion rotation)
        => rotation * Quaternion.Euler(UnityEngine.Random.Range(-Spread, Spread), 0, UnityEngine.Random.Range(-Spread, Spread));
}
