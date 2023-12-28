using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class JiggleChainConstraintBinder : AnimationJobBinder<JiggleChainConstraintJob, JiggleChainConstraintData>
{
    public override JiggleChainConstraintJob Create(Animator animator, ref JiggleChainConstraintData data, Component component)
    {
        List<Transform> chain = new List<Transform>();
        Transform tmp = data.Tip;
        while (tmp != data.Root)
        {
            chain.Add(tmp);
            tmp = tmp.parent;
        }
        chain.Reverse();

        var job = new JiggleChainConstraintJob();

        job.Chain = new NativeArray<ReadWriteTransformHandle>(chain.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        job.DynamicTargetAim = new NativeArray<JiggleChainConstraintJob.DynamicTarget>(chain.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        job.DynamicTargetRoll = new NativeArray<JiggleChainConstraintJob.DynamicTarget>(chain.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        job.LocalAimDirection = math.normalize(data.LocalAimVector);
        job.LocalUpDirection = math.normalize(data.LocalUpVector);

        float3 localAimOffset = job.LocalAimDirection * data.DynamicOffset;
        float3 localUpOffset = job.LocalUpDirection * data.DynamicOffset;
        for (int i = 0; i < chain.Count; ++i)
        {
            job.Chain[i] = ReadWriteTransformHandle.Bind(animator, chain[i]);
            float4x4 tx = float4x4.TRS(chain[i].position, chain[i].rotation, new float3(1f));

            job.DynamicTargetAim[i] = new JiggleChainConstraintJob.DynamicTarget() { Position = math.transform(tx, localAimOffset), Velocity = Vector3.zero };
            job.DynamicTargetRoll[i] = new JiggleChainConstraintJob.DynamicTarget() { Position = math.transform(tx, localUpOffset), Velocity = Vector3.zero };
        }

        // Bind dynamic properties
        job.MassProperty =          FloatProperty.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.Mass)));
        job.StiffnessProperty =     FloatProperty.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.Stiffness)));
        job.DynamicOffsetProperty = FloatProperty.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.DynamicOffset)));
        job.RollEnabledProperty =   BoolProperty.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.RollEnabled)));
        job.GravityProperty =       Vector3Property.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.Gravity)));
        job.MotionDecayProperty =   FloatProperty.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.MotionDecay)));
        job.DampingProperty =       FloatProperty.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.Damping)));

        job.DecayLogRange = new LogRangeConverter(0, 0.1f, 1);

        return job;
    }

    public override void Destroy(JiggleChainConstraintJob job)
    {
        job.Chain.Dispose();
        job.DynamicTargetAim.Dispose();
        job.DynamicTargetRoll.Dispose();
    }
}
