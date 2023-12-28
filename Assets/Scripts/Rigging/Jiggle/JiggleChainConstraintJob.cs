using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using System;

//[BurstCompile]
public struct JiggleChainConstraintJob : IWeightedAnimationJob
{
    const float EPSILON = 1e-6f;
    const float FORCE_MULTIPLIER = 62f;
    const float FIXED_DELTA = 1f / 60f; // 60Hz fixed timestep.

    public FloatProperty jobWeight { get; set; }

    public float3 LocalAimDirection;
    public float3 LocalUpDirection;

    public NativeArray<ReadWriteTransformHandle> Chain;
    public NativeArray<DynamicTarget> DynamicTargetAim;
    public NativeArray<DynamicTarget> DynamicTargetRoll;

    // User input properties.
    public FloatProperty MassProperty;
    public FloatProperty StiffnessProperty;
    public FloatProperty DynamicOffsetProperty;
    public BoolProperty RollEnabledProperty;
    public Vector3Property GravityProperty;
    public FloatProperty MotionDecayProperty;
    public FloatProperty DampingProperty;
    public LogRangeConverter DecayLogRange;

    private SimulationProperties _simulationProperties;

    public struct SimulationProperties
    {
        public float Mass;         // Particle mass.
        public float Decay;        // Controls overshoot by removing some motion momentum.
        public float Damping;      // Velocity damping factor.
        public float Stiffness;    // Distance constraint stiffness.
        public float RestDistance; // Dynamic offset distance used as rest distance for constraint.
    }

    public struct DynamicTarget
    {
        public float3 Position;
        public float3 Velocity;

        public void Reset(float3 position)
        {
            this.Position = position;
            Velocity = float3.zero;
        }

        public void ApplyDistanceConstraint(ref float3 dynamicPos, float3 pinnedPos, float stiffness, float restDistance)
        {
            // Geometric distance constraint considering a pinned position. All correction to respect constraint is applied to
            // dynamicPos. Since we are not considering the mass of both particles, gravity will not affect the
            // constraint behavior.

            float3 diff = pinnedPos - dynamicPos;
            float sqLength = math.lengthsq(diff);
            if (sqLength > EPSILON)
            {
                float length = math.sqrt(sqLength);
                dynamicPos += diff * stiffness * ((length - restDistance) / length);
            }
        }

        public void Update(float3 targetRoot, float3 target, float3 forces, in SimulationProperties properties)
        {
            // Solver based from the work of Mathias Muller and al. "Position Based Dynamics",
            // 3rd Workshop in Virtual Reality Interactions and Physical Simulation "VRIPHYS" (2006),
            // AGEIA ( Section 3.3 )

            float3 acceleration = (forces - properties.Damping * Velocity) / properties.Mass;
            Velocity += acceleration * FIXED_DELTA;
            Velocity *= properties.Decay;
            float3 prevPosition = Position;

            Position += Velocity * FIXED_DELTA;
            ApplyDistanceConstraint(ref Position, targetRoot, properties.Stiffness, properties.RestDistance);
            ApplyDistanceConstraint(ref Position, target, properties.Stiffness, 0f);

            Velocity = (Position - prevPosition) / FIXED_DELTA;
        }
    }

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        float w = jobWeight.Get(stream);
        bool doRoll = RollEnabledProperty.Get(stream);

        _simulationProperties = new SimulationProperties
        {
            Mass = MassProperty.Get(stream),
            Decay = AnimationRuntimeUtils.Square(1f - DecayLogRange.ToRange(MotionDecayProperty.Get(stream))),
            Damping = 1f - DampingProperty.Get(stream),
            Stiffness = AnimationRuntimeUtils.Square(StiffnessProperty.Get(stream)),
            RestDistance = DynamicOffsetProperty.Get(stream),
        };

        float3 gravity = GravityProperty.Get(stream);
        float3 forces = (gravity * _simulationProperties.Mass) * FORCE_MULTIPLIER;

        for (int i = 0, count = Chain.Length; i < count; ++i)
        {

            ReadWriteTransformHandle chainHandle = Chain[i];
            chainHandle.GetGlobalTR(stream, out Vector3 chainWPos, out Quaternion chainWRot);

            float4x4 tx = float4x4.TRS(chainWPos, chainWRot, new float3(1f));
            float3 aimTarget = math.transform(tx, LocalAimDirection * _simulationProperties.RestDistance);
            float3 rollTarget = math.transform(tx, LocalUpDirection * _simulationProperties.RestDistance);

            var dynamicAim = DynamicTargetAim[i];
            var dynamicRoll = DynamicTargetRoll[i];

            float streamDeltaTime = math.abs(stream.deltaTime);
            if (w > 0f && streamDeltaTime > 0f)
            {
                float3 txTranslation = chainWPos;
                quaternion txRotation = chainWRot;

                while (streamDeltaTime > 0f)
                {
                    dynamicAim.Update(txTranslation, aimTarget, forces, _simulationProperties);
                    if (doRoll)
                    {
                        // Roll particles are not affected by external forces to prevent weird configurations
                        dynamicRoll.Update(txTranslation, rollTarget, float3.zero, _simulationProperties);
                    }

                    streamDeltaTime -= FIXED_DELTA;
                }

                // Compute aimDeltaRot in all axis except roll
                float3 rotMask = math.abs(LocalAimDirection);
                quaternion aimDeltaRot = Project(
                    txRotation,
                    FromTo(aimTarget - txTranslation, dynamicAim.Position - txTranslation),
                    new float3(1f) - rotMask
                    );

                // Compute rollDeltaRot in roll axis only
                quaternion rollDeltaRot = doRoll ? Project(
                    txRotation,
                    FromTo(rollTarget - txTranslation, dynamicRoll.Position - txTranslation),
                    rotMask
                    ) : quaternion.identity;

                Chain[i].SetRotation(
                    stream,
                    math.slerp(Chain[i].GetRotation(stream), math.mul(rollDeltaRot, math.mul(aimDeltaRot, txRotation)), w)
                    );
            }
            else
            {
                dynamicAim.Reset(aimTarget);
                dynamicRoll.Reset(rollTarget);
                AnimationRuntimeUtils.PassThrough(stream, chainHandle);
            }

            DynamicTargetAim[i] = dynamicAim;
            DynamicTargetRoll[i] = dynamicRoll;
            Chain[i] = chainHandle;
        }
    }

    private static quaternion Project(quaternion rot, quaternion deltaRot, float3 mask)
    {
        quaternion invRot = math.inverse(rot);
        quaternion tmp = math.mul(invRot, math.mul(deltaRot, rot));
        tmp.value.x *= mask.x;
        tmp.value.y *= mask.y;
        tmp.value.z *= mask.z;
        return math.mul(rot, math.mul(tmp, invRot));
    }

    private static quaternion FromTo(float3 from, float3 to)
    {
        float theta = math.dot(math.normalize(from), math.normalize(to));
        if (theta >= 1f)
            return quaternion.identity;

        if (theta <= -1f)
        {
            float3 axis = math.cross(from, new float3(1f, 0f, 0f));
            if (math.lengthsq(axis) == 0f)
                axis = math.cross(from, math.up());

            return quaternion.AxisAngle(axis, (float)math.PI);
        }

        return quaternion.AxisAngle(math.normalize(math.cross(from, to)), math.acos(theta));
    }
}
