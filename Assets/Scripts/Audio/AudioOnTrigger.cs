using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks.Linq;

[HideMonoScript]
public class AudioOnTrigger : AudioOnEnterBase
{
    protected override async UniTaskVoid OnEnterAsync(CancellationToken cancellationToken)
    {
        await _target.GetAsyncTriggerEnterTrigger().ForEachAsync(collider =>
        {
            if (((1 << collider.gameObject.layer) & _layerMask) != 0)
                _audioSource.PlayOneShot(_playlist);

        }, cancellationToken);
    }
}
