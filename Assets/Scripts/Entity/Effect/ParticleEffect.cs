using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Pool;

public class ParticleEffect : IEffectAsync, IEffectValid, IEffectTag
{
    string IEffectTag.Tag => Tag;

    public GameObject ParticlePrefab;
    public string Tag;
    [MinValue(0)]
    public float Duration = 5;

    private ParticlePool _particlePool;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        _particlePool = new ParticlePool(ParticlePrefab, particleSystem =>
        {
            if (particleSystem.isPlaying)
                particleSystem.Stop();

            var shape = particleSystem.shape;
            if (shape.shapeType != ParticleSystemShapeType.Rectangle)
            {
                shape.shapeType = ParticleSystemShapeType.Rectangle;
                shape.scale = Vector3.one;
            }

            var main = particleSystem.main;
            main.playOnAwake = false;
            main.loop = false;
        });
    }

    public async UniTask UpdateAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender)
    {
        var particleSystem = _particlePool.Get();

        var effectorTransform = effector.GameObject.transform;
        var particleTransform = particleSystem.gameObject.transform;

        particleTransform.parent = effectorTransform;
        particleTransform.localScale = Vector3.one;

        // Set shape.
        var shape = particleSystem.shape;
        float defaultArea = shape.scale.x * shape.scale.y;
        float area = defaultArea;

        if (effector.GameObject.TryGetComponent(out Collider collider))
        {
            Bounds bounds = collider.bounds;
            area = bounds.size.x * bounds.size.z;

            particleTransform.position = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            shape.scale = new Vector3(bounds.size.x, bounds.size.z, 1);
        }
        else
            particleTransform.position = effectorTransform.position;

        // Set duration.
        var main = particleSystem.main;
        main.duration = Mathf.Max(0, Duration - main.startLifetime.mode switch
        {
            ParticleSystemCurveMode.Constant => main.startLifetime.constant,
            ParticleSystemCurveMode.TwoConstants => main.startLifetime.constantMax,
            _ => 0,
        });

        // Set emission.
        var emission = particleSystem.emission;
        var rateOverTimeMode = emission.rateOverTime.mode;
        float defaultRateOverTime = 0;
        switch (rateOverTimeMode) // Modify based on surface area.
        {
            case ParticleSystemCurveMode.Constant:
            case ParticleSystemCurveMode.TwoConstants:
                defaultRateOverTime = emission.rateOverTime.constant;
                emission.rateOverTime = defaultRateOverTime * (area / defaultArea);   // Multiply by the area difference.
                break;
        }

        particleSystem.Play();

        try
        {
           await UniTask.WaitForSeconds(Duration, cancellationToken: cancellationToken);
        }
        catch { }

        switch (rateOverTimeMode) // Set to default.
        {
            case ParticleSystemCurveMode.Constant:
            case ParticleSystemCurveMode.TwoConstants:
                emission.rateOverTime = defaultRateOverTime;
                break;
        }

        particleTransform.parent = null;

        _particlePool.Release(particleSystem);
    }

    public bool IsValid(IEffector effector, IEffect effect, GameObject sender)
    {
        if (effector.Effects.Any(x => x is ParticleEffect particleEffect && particleEffect.Tag == Tag))
            return false;

        return true;
    }
}
