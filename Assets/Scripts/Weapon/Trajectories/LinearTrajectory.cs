using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearTrajectory : ITrajectory
{
    [SuffixLabel("m/s", overlay: true)]
    public float Speed = 20f;

    public void Initialize(Rigidbody rigidbody)
    {
        rigidbody.AddRelativeForce(Vector3.right * Speed, ForceMode.VelocityChange);
    }
}
