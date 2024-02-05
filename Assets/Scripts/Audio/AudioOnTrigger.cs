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
    protected AudioSource _audioSource;
    [SerializeField]
    protected GameObject _target;
    [SerializeField]
    protected LayerMask _layerMask = ~0;

    [SerializeField]
    protected Playlist _playlist;

    private CancellationTokenSource _cancellationTokenSource;

    private void OnEnable()
    {
        if (_target == null)
            return;

        CTSUtility.Reset(ref _cancellationTokenSource);

        _target.GetAsyncTriggerEnterTrigger().ForEachAsync(collider =>
        {
            if (((1 << collider.gameObject.layer) & _layerMask) != 0)
                _audioSource.PlayOneShot(_playlist);

        }, _cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }
}
