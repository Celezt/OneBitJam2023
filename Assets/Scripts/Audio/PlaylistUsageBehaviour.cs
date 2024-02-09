using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

[HideMonoScript]
public class PlaylistUsageBehaviour : MonoBehaviour
{
    public Playlist BeginPlaylist => _beginPlaylist;
    public Playlist UsingPlaylist => _usingPlaylist;
    public Playlist EndPlaylist => _endPlaylist;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField, Space(4)]
    private Playlist _beginPlaylist;
    [SerializeField]
    private Playlist _usingPlaylist;
    [SerializeField]
    private Playlist _endPlaylist;

    private CancellationTokenSource _cancellationTokenSource;

    public void BeginPlay()
    {
        CTSUtility.Reset(ref _cancellationTokenSource);
        OnUsageAsync(_cancellationTokenSource.Token).Forget();
    }

    public void EndPlay()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
        _audioSource.Play(_endPlaylist);
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private async UniTaskVoid OnUsageAsync(CancellationToken cancellationToken)
    {
        await _audioSource.PlayAsync(_beginPlaylist, cancellationToken);

        if (_usingPlaylist.Count == 1)  // Loop if there is only one clip.
        {
            bool defaultLoop = _audioSource.loop;
            _audioSource.loop = true;
            _audioSource.Play(_usingPlaylist);

            try
            {
                await UniTask.WaitUntilCanceled(cancellationToken);
            }
            catch { }

            _audioSource.loop = defaultLoop;
        }
        else
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _audioSource.PlayAsync(_usingPlaylist, cancellationToken);
            }
        }
    }
}
