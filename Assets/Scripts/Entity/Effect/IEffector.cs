using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IEffector
{
    public GameObject GameObject { get; }
    public IEnumerable<IEffectAsync> Effects { get; }
    public IEnumerable<IEffectorProperty> Properties { get; }

    public event Action<IEffector, IEffect, GameObject> OnEffectAddedCallback;
    public event Action<IEffector, IEffect> OnEffectRemovedCallback;

    public bool AddEffect(IEffect effect, GameObject sender);
    public bool AddEffects(IEnumerable<IEffect> effects, GameObject sender);
    public bool RemoveEffect(IEffectAsync effect);

    public void AddProperty(IEffectorProperty property);
    public void RemoveProperty(IEffectorProperty property);
}
