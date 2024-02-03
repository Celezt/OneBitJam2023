using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImmunityEffectorProperty : IEffectorProperty, IEffectValid
{
    public string Tag => _tag;

    [SerializeField]
    private string _tag;

    public void OnEnable(IEffector effector)
    {

    }

    public void OnDisable(IEffector effector)
    {

    }

    public bool IsValid(IEffector effector, IEffect effect, GameObject sender)
    {
        if (effect is IEffectTag effectTag && effectTag.Tag == Tag)
            return false;

        return true;
    }
}
