using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class AudioEffect : IEffectAsync, IEffectValid, IEffectTag
{
    string IEffectTag.Tag => Tag;

    public GameObject AudioSourcePrefab;
    public string Tag;
    [MinValue(0)]
    public float Duration = 5;
    [Indent, MinValue(0)]
    public float FadeDuration = 0.5f;
    public Playlist Playlist;

    private AudioPool _audioPool;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        _audioPool = new AudioPool(AudioSourcePrefab);
    }

    public async UniTask UpdateAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender)
    {
        if (Duration - FadeDuration <= 0)
            return;

        var audioSource = _audioPool.Get();

        var effectorTransform = effector.GameObject.transform;
        var audioSourceTransform = audioSource.gameObject.transform;

        audioSourceTransform.parent = effectorTransform;
        audioSourceTransform.localPosition = Vector3.zero;

        audioSource.Play(Playlist);

        await UniTask.WaitForSeconds(Duration - FadeDuration, cancellationToken: cancellationToken);
        await audioSource.FadeOut(FadeDuration, cancellationToken);

        audioSource.Stop();

        audioSourceTransform.parent = null;

        _audioPool.Release(audioSource);
    }

    public bool IsValid(IEffector effector, IEffect effect, GameObject sender)
    {
        if (effector.Effects.Any(x => x is AudioEffect audioEffect && audioEffect.Tag == Tag))
            return false;

        return true;
    }

    private AudioSource CreateAudioSource()
    {
        if (AudioSourcePrefab == null)
            return null;

        GameObject audioSourceObject = UnityEngine.Object.Instantiate(AudioSourcePrefab);
        AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();

        audioSource.Stop();
        audioSource.playOnAwake = false;

        return audioSource;
    }
}
