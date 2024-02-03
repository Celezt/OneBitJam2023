using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectorProperty
{
    public void OnEnable(IEffector effector);
    public void OnDisable(IEffector effector);
}
