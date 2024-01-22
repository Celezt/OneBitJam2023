using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleEffect : IEffectAsync
{
    private static Dictionary<string, ObjectPool<ParticleSystem>> _particlePools = new ();

    public GameObject ParticlePrefab;
    public string Element;
    [MinValue(0)]
    public float Duration = 5;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {

    }

    public async UniTask UpdateAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender)
    {
        if (!_particlePools.TryGetValue(Element, out ObjectPool<ParticleSystem> pool))
        {
            _particlePools[Element] = pool = new ObjectPool<ParticleSystem>(
                createFunc: () => CreateParticleSystem(),
                actionOnGet: particle =>
                {
                    particle.gameObject.SetActive(true);
                },
                actionOnRelease: particle =>
                {
                    particle.gameObject.SetActive(false);
                },
                actionOnDestroy: particle =>
                {
                    if (particle != null)
                        UnityEngine.Object.Destroy(particle.gameObject);
                },
                collectionCheck: true
                );
        }

        var particleSystem = pool.Get();

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
        float defaultRateOverTime = 0;
        var rateOverTimeMode = emission.rateOverTime.mode;
        switch (rateOverTimeMode) // Modify based on surface area.
        {
            case ParticleSystemCurveMode.Constant:
            case ParticleSystemCurveMode.TwoConstants:
                defaultRateOverTime = emission.rateOverTime.constant;
                emission.rateOverTime = defaultRateOverTime * (area / defaultArea);   // Multiply by the area difference.
                break;
        }

        particleSystem.Play();

        await UniTask.WaitForSeconds(Duration, cancellationToken: cancellationToken);

        switch (rateOverTimeMode) // Set to default.
        {
            case ParticleSystemCurveMode.Constant:
            case ParticleSystemCurveMode.TwoConstants:
                emission.rateOverTime = defaultRateOverTime;
                break;
        }

        particleTransform.parent = null;

        pool.Release(particleSystem);
    }

    public bool IsValid(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
        => !effects.Any(x => x is ParticleEffect effect && effect.Element == Element);

    private ParticleSystem CreateParticleSystem()
    {
        if (ParticlePrefab == null)
            return null;

        GameObject particleObject = UnityEngine.Object.Instantiate(ParticlePrefab);
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

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

        return particleSystem;
    }
}
