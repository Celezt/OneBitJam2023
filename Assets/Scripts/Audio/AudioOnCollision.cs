using Cysharp.Threading.Tasks;
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
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private LayerMask _layerMask = ~0;
    [SerializeField]
    private Playlist _playList;

    private void Start()
    {
        if (_target == null)
            return;

        OnCollisionEnterAsync(destroyCancellationToken).Forget();
    }

    private async UniTaskVoid OnCollisionEnterAsync(CancellationToken cancellationToken)
    {
        var handler = _target.GetAsyncCollisionEnterTrigger().GetOnCollisionEnterAsyncHandler(cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            Collision collision = await handler.OnCollisionEnterAsync();

            if (((1 << collision.collider.gameObject.layer) & _layerMask) != 0)
            {
                if (_audioSource != null)
                    _audioSource.PlayOneShot(_playList);
            }
        }
    }
}
