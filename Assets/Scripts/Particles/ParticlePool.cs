using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public struct ParticlePool
{
    private readonly static Dictionary<GameObject, ObjectPool<ParticleSystem>> _particleSystemPools = new();

    public bool IsEmpty => _prefab == null;
    public GameObject Prefab => _prefab;

    private readonly GameObject _prefab;

    static ParticlePool()
    {
        SceneManager.activeSceneChanged += (newScene, oldScene) =>
        {
            foreach (var pools in _particleSystemPools.Values)
                pools.Clear();
        };
    }

    public ParticlePool(GameObject particlePrefab, Action<ParticleSystem> onCreateCallback = null)
    {
        _prefab = particlePrefab;

        Assert.IsNotNull(_prefab.GetComponent<ParticleSystem>());

        if (!_particleSystemPools.ContainsKey(particlePrefab))
        {
            ParticleSystem CreateParticleSystem()
            {
                if (particlePrefab == null)
                    return null;

                GameObject particleObject = UnityEngine.Object.Instantiate(particlePrefab);
                ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

                onCreateCallback?.Invoke(particleSystem);

                return particleSystem;
            }

            void OnGetParticleSystem(ParticleSystem particleSystem)
            {
                particleSystem.gameObject.SetActive(true);
            }

            void OnReleaseParticleSystem(ParticleSystem particleSystem)
            {
                particleSystem.Stop();
                particleSystem.gameObject.SetActive(false);
            }

            void OnDestroyParticleSystem(ParticleSystem particleSystem)
            {
                if (particleSystem != null)
                    UnityEngine.Object.Destroy(particleSystem.gameObject);
            }

            _particleSystemPools[particlePrefab] = new ObjectPool<ParticleSystem>(
                createFunc: CreateParticleSystem,
                actionOnGet: OnGetParticleSystem,
                actionOnRelease: OnReleaseParticleSystem,
                actionOnDestroy: OnDestroyParticleSystem,
                collectionCheck: true);
        }
    }

    public ParticleSystem Get()
    {
        if (!_prefab)
        {
            Debug.LogError("Particle Pool does not contain any prefab reference");
            return null;
        }

        var particleSystem = _particleSystemPools[_prefab].Get();
        return particleSystem;
    }

    public void Release(ParticleSystem particleSystem)
    {
        if (!_prefab)
        {
            Debug.LogError("Particle Pool does not contain any prefab reference");
            return;
        }

        _particleSystemPools[_prefab].Release(particleSystem);
    }

    public static void Release(GameObject particlePrefab, ParticleSystem particleSystem)
    {
        _particleSystemPools[particlePrefab].Release(particleSystem);
    }

    public static ParticleSystem Get(GameObject particlePrefab)
    {
        ParticlePool particlePool = new(particlePrefab);
        return particlePool.Get();
    }

    public static void Remove(GameObject particlePrefab)
    {
        _particleSystemPools.Remove(particlePrefab);
    }
}
