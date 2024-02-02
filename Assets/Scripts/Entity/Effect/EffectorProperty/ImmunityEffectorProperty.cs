using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmunityEffectorProperty : IEffectorProperty, IImmunity
{
    public string Tag => _tag;

    [SerializeField]
    private string _tag;

    public void Initialize(IEffector effector, IEnumerable<IEffectorProperty> properties)
    {

    }
}
