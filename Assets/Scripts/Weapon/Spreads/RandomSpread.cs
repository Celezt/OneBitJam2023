using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpread : ISpread
{
    [SuffixLabel("degree", overlay: true)]
    public float Spread = 4;
    public Vector3 Scale = new Vector3(0, 1, 1);

    public Quaternion Rotation(Quaternion rotation)
        => rotation * Quaternion.Euler
        (
            Scale.x * UnityEngine.Random.Range(-Spread, Spread), 
            Scale.y * UnityEngine.Random.Range(-Spread, Spread), 
            Scale.z * UnityEngine.Random.Range(-Spread, Spread)
        );
}
