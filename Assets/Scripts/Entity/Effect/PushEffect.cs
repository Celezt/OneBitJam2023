using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushEffect : IEffect
{
    public float Force = 100.0f;
    [Indent]
    public float RagdollMultiplier = 1.0f;
    public float Radius = 3f;
    public float UpwardsModifier = 0f;
    public LocationMode Mode = LocationMode.Velocity;
    [ShowIf(nameof(Mode), LocationMode.Velocity), Indent]
    public float Distance = 2;

    public enum LocationMode
    {
        Position,
        Velocity,
    }

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        if (!effector.GameObject.TryGetComponent(out Rigidbody effectorRigidbody))
            return;

        Vector3 position;
        switch (Mode)
        {
            case LocationMode.Velocity when sender.TryGetComponent(out Rigidbody senderRigidbody):
                Vector3 direction = senderRigidbody.velocity.normalized;
                position = sender.transform.position - direction * Distance;
                break;
            default:
                position = sender.transform.position;
                break;
        }

        if (effectorRigidbody != null)
            effectorRigidbody.AddExplosionForce(Force, position, Radius, UpwardsModifier, ForceMode.Impulse);

        // Add force to the ragdoll if it exist.
        if (effector.GameObject.TryGetComponent(out IRagdoll ragdoll))
            ragdoll.Push(Force * RagdollMultiplier, position, Radius, UpwardsModifier);
    }
}
