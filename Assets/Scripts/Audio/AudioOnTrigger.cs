using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks.Linq;

[HideMonoScript]
public class AudioOnTrigger : MonoBehaviour
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
        await _target.GetAsyncTriggerEnterTrigger().ForEachAsync(collider =>
        {
            if (((1 << collider.gameObject.layer) & _layerMask) != 0)
            {
                if (_audioSource != null)
                    _audioSource.PlayOneShot(_playList);
            }
        }, cancellationToken);
    }
}
