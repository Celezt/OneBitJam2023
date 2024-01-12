using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDetonation : IDetonation
{
    public float Cooldown => _cooldown;

    [MinValue(0)]
    public int Amount = 1;

    [SerializeField, MinValue(0)]
    private float _cooldown = 0.2f;

    public void Initialize(IDetonator handler)
    {
        for (int i = 0; i < Amount; i++)
        {
            handler.Trigger();
        }
    }
}
