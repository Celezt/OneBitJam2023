using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushEffect : IEffect
{
    public float Force = 100.0f;
    public float Radius = 3f;
    public float UpwardsModifier = 0f;
    public LocationMode Mode = LocationMode.Position;
    [ShowIf(nameof(Mode), LocationMode.Velocity), Indent]
    public float Distance = 2;

    public enum LocationMode
    {
        Position,
        Velocity,
    }

    public void Effect(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        if (!effector.GameObject.TryGetComponentInChildren(out IPushable pushable))
            return;

        Vector3 location;
        if (Mode == LocationMode.Velocity && sender.TryGetComponentInChildren(out Rigidbody rigidbody))
        {
            Vector3 direction = rigidbody.velocity.normalized;
            location = sender.transform.position - direction * Distance;
        }
        else
            location = sender.transform.position;

        pushable.Push(Force, location, Radius, UpwardsModifier);

        // Add force to the ragdoll if it exist.
        if (effector.GameObject.TryGetComponentInChildren(out Ragdoll ragdoll))
            ragdoll.Push(Force, location, Radius, UpwardsModifier);
    }
}
