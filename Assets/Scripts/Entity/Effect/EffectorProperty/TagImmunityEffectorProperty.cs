using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagImmunityEffectorProperty : IEffectorProperty, IElementalImmunity
{
    public string Element => _element;

    [SerializeField]
    private string _element;

    public void Initialize(IEffector effector, IEnumerable<IEffectorProperty> properties)
    {

    }
}
