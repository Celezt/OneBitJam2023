using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public interface IEffectSingle : IEffect
{
    public void Effect(IEffector effector, IEnumerable<IEffectAsync> effects);
}
