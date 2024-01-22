using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectorProperty
{
    public void Initialize(IEffector effector, IEnumerable<IEffectorProperty> properties);
}
