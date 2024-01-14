using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class AudioOnEnterBase : MonoBehaviour
{
    [SerializeField]
    protected AudioSource _audioSource;
    [SerializeField]
    protected GameObject _target;
    [SerializeField]
    protected LayerMask _layerMask = ~0;
    [SerializeField, PropertyOrder(int.MaxValue)]
    protected Playlist _playlist;

    protected abstract UniTaskVoid OnEnterAsync(CancellationToken cancellationToken);

    private void Start()
    {
        if (_target == null)
            return;

        OnEnterAsync(destroyCancellationToken).Forget();
    }

}
