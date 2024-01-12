using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public interface IEffect
{
    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender);
}
