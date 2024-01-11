using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrajectory : ITrajectoryBase
{
    public void Initialize(Rigidbody rigidbody);
}
