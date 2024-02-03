using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectValid
{
    public bool IsValid(IEffector effector, IEffect effect, GameObject sender);
}
