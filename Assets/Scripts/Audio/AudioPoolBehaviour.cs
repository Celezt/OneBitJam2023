using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[HideMonoScript]
public class AudioPoolBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _audioSourcePrefab;
    [SerializeField]
    public bool _attachToParent = true;

    [SerializeField]
    public bool _customDuration;
    [SerializeField, MinValue(0), ShowIf(nameof(_customDuration))]
    public float _duration = 5;
    [SerializeField, Indent, MinValue(0), ShowIf(nameof(_customDuration))]
    public float _fadeDuration = 0.5f;

    [SerializeField]
    public Playlist _playlist;

    private AudioPool _audioPool;
    private CancellationTokenSource _cancellationTokenSource;

    public void Play()
    {
        CTSUtility.Reset(ref _cancellationTokenSource);
        PlayAudioAsync(_cancellationTokenSource.Token).Forget();
    }

    private void Start()
    {
        _audioPool = new AudioPool(_audioSourcePrefab);
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private async UniTaskVoid PlayAudioAsync(CancellationToken cancellationToken)
    {
        if (_customDuration && _duration - _fadeDuration <= 0)
            return;

        var audioSource = _audioPool.Get();

        var audioSourceTransform = audioSource.gameObject.transform;

        if (_attachToParent)
        {
            audioSourceTransform.parent = transform;
            audioSourceTransform.localPosition = Vector3.zero;
        }
        else
            audioSourceTransform.position = transform.position;

        var clip = audioSource.Play(_playlist);

        float duration = _customDuration ? _duration - _fadeDuration : clip.AudioClip.length;
        await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);

        if (_customDuration && _fadeDuration > 0)
            await audioSource.FadeOut(_fadeDuration, cancellationToken);

        audioSource.Stop();

        if (_attachToParent)
            audioSourceTransform.parent = null;

        _audioPool.Release(audioSource);
    }
}
