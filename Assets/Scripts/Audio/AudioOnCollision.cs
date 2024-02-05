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
public class AudioOnCollision : MonoBehaviour
{
    [SerializeField]
    protected AudioSource _audioSource;
    [SerializeField]
    protected GameObject _target;
    [SerializeField]
    protected LayerMask _layerMask = ~0;

    [SerializeField]
    private bool _scaleOfSpeed = false;
    [SerializeField, Indent, ShowIf(nameof(_scaleOfSpeed))]
    private float _maxSpeed = 6.0f;

    [SerializeField]
    protected Playlist _playlist;

    private CancellationTokenSource _cancellationTokenSource;

    private void OnEnable()
    {
        if (_target == null)
            return;

        CTSUtility.Reset(ref _cancellationTokenSource);

        _target.GetAsyncCollisionEnterTrigger().ForEachAsync(collision =>
        {
            if (((1 << collision.gameObject.layer) & _layerMask) != 0)
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
        }, _cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }
}
