using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable
{
    public void Push(float force, Vector3 position, float radius, float upwardsModifier = 0f);
}
