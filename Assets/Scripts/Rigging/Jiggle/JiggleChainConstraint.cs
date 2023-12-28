using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[Serializable]
public struct JiggleChainConstraintData : IAnimationJobData
{
    [SyncSceneToStream] public Transform Root;
    public Transform Tip;

    [Header("Properties")]
    [SyncSceneToStream] public float Mass;
    [SyncSceneToStream, Range(0f, 1f)] public float Stiffness;
    [SyncSceneToStream, Min(0.1f)] public float DynamicOffset;

    public Vector3 LocalAimVector;
    public Vector3 LocalUpVector;
    [SyncSceneToStream] public bool RollEnabled;

    [Header("Forces")]
    [SyncSceneToStream] public Vector3 Gravity;

    [Header("Simulation Settings")]
    [SyncSceneToStream, Range(0f, 1f)] public float Damping;
    [SyncSceneToStream, Range(0f, 1f)] public float MotionDecay;

    public bool IsValid()
    {
        if (Root == null || Tip == null || Root == Tip)
            return false;

        Transform tmp = Tip;
        while (tmp != null && tmp != Root)
            tmp = tmp.parent;

        return (tmp == Root);
    }

    public void SetDefaultValues()
    {
        Root = null;
        Tip = null;
        Mass = 1f;
        Stiffness = 0.5f;
        DynamicOffset = 4f;
        LocalAimVector = new Vector3(1f, 0f, 0f);
        LocalUpVector = new Vector3(0f, 1f, 0f);
        RollEnabled = false;
        Gravity = Vector3.zero;
        MotionDecay = 0.1f;
        Damping = 0.0f;
    }
}

[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Jiggle Chain Constraint"), HideMonoScript]
public class JiggleChainConstraint : RigConstraint<JiggleChainConstraintJob, JiggleChainConstraintData, JiggleChainConstraintBinder>
{ }
