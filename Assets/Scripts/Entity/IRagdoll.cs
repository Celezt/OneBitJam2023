using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRagdoll
{
    public void OnEnableRagdoll();
    public void OnDisableRagdoll();
    public void Push(float force, Vector3 position, float radius, float upwardsModifier = 1.0f);
}
