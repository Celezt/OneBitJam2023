using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[HideMonoScript]
public class AudioOnCollision : AudioOnEnterBase
{
    [SerializeField]
    private bool _scaleOfSpeed = false;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfSpeed))]
    private float _maxSpeed = 6.0f;

    protected override async UniTaskVoid OnEnterAsync(CancellationToken cancellationToken)
    {
        await _target.GetAsyncCollisionEnterTrigger().ForEachAsync(collision =>
        {
            if (((1 << collision.collider.gameObject.layer) & _layerMask) != 0)
            {
                if (_scaleOfSpeed)
                {
                    float speed = collision.relativeVelocity.magnitude;
                    float interval = Mathf.Clamp01(speed / _maxSpeed);
                    _audioSource.PlayOneShot(_playlist, interval);
                }
                else
                    _audioSource.PlayOneShot(_playlist);
            }
        }, cancellationToken);
    }
}
