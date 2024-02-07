using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

[HideMonoScript]
public class ParticlePoolBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _particleSystemPrefab;
    [SerializeField]
    public bool _attachToParent = true;

    [SerializeField, Space(4)]
    public bool _customDuration;
    [SerializeField, MinValue(0), ShowIf(nameof(_customDuration))]
    public float _duration = 5;

    private ParticlePool _particlePool;

    public void Play()
    {
        PlayParticleSystemAsync().Forget();
    }

    private void Start()
    {
        _particlePool = new ParticlePool(_particleSystemPrefab);
    }

    private async UniTaskVoid PlayParticleSystemAsync()
    {
        if (_customDuration && _duration <= 0)
            return;

        var parentParticleSystem = _particlePool.Get();
        var destroyCancellationToken = parentParticleSystem.GetCancellationTokenOnDestroy();

        float maxDuration = 0;
        if (!_customDuration)
        {
            ParticleSystem[] particleSystems = parentParticleSystem.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                var main = particleSystem.main;

                if (main.duration > maxDuration)
                    maxDuration = main.duration;
            }
        }
        else
            maxDuration = _duration;

        var particleSystemTransform = parentParticleSystem.gameObject.transform;

        if (_attachToParent)
        {
            particleSystemTransform.parent = transform;
            particleSystemTransform.localPosition = Vector3.zero;
        }
        else
            particleSystemTransform.position = transform.position;

        await UniTask.WaitForSeconds(maxDuration, cancellationToken: destroyCancellationToken);

        if (_attachToParent)
            particleSystemTransform.parent = null;

        _particlePool.Release(parentParticleSystem);
    }
}
