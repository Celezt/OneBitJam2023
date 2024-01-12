using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetonation
{
    public float Cooldown { get; }

    public void Initialize(IDetonator handler);
}
