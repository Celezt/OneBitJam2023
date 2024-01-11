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
        rigidbody.AddRelativeForce(new Vector3(Speed, 0, 0), ForceMode.VelocityChange);
    }
}
