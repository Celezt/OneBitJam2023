using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class AudioEffect : IEffectAsync
{
    private readonly static Dictionary<string, Dictionary<GameObject, ObjectPool<AudioSource>>> _audioSourcePools = new();

    public GameObject AudioSourcePrefab;
    public string Tag;
    [MinValue(0)]
    public float Duration = 5;
    [Indent, MinValue(0)]
    public float FadeDuration = 0.5f;
    public Playlist Playlist;

    public void Initialize(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {

    }

    public async UniTask UpdateAsync(IEffector effector, IEnumerable<IEffectAsync> effects, CancellationToken cancellationToken, GameObject sender)
    {
        if (Duration - FadeDuration <= 0)
            return;

        if (!_audioSourcePools.TryGetValue(Tag, out var poolsByTag))
            _audioSourcePools[Tag] = poolsByTag = new();

        if (!poolsByTag.TryGetValue(AudioSourcePrefab, out ObjectPool<AudioSource> pool))
        {
            poolsByTag[AudioSourcePrefab] = pool = new ObjectPool<AudioSource>(
                createFunc: () => CreateAudioSource(),
                actionOnGet: audioSource =>
                {
                    audioSource.gameObject.SetActive(true);
                },
                actionOnRelease: audioSource =>
                {
                    audioSource.gameObject.SetActive(false);
                },
                actionOnDestroy: audioSource =>
                {
                    if (audioSource != null)
                        UnityEngine.Object.Destroy(audioSource.gameObject);
                },
                collectionCheck: true
                );
        }

        var audioSource = pool.Get();

        var effectorTransform = effector.GameObject.transform;
        var audioSourceTransform = audioSource.gameObject.transform;

        audioSourceTransform.parent = effectorTransform;
        audioSourceTransform.localPosition = Vector3.zero;

        audioSource.Play(Playlist);

        await UniTask.WaitForSeconds(Duration - FadeDuration, cancellationToken: cancellationToken);
        await audioSource.FadeOut(FadeDuration, cancellationToken);

        audioSource.Stop();

        audioSourceTransform.parent = null;

        pool.Release(audioSource);
    }

    public bool IsValid(IEffector effector, IEnumerable<IEffectAsync> effects, GameObject sender)
    {
        if (effector.Properties.Any(x => x is IImmunity immunity && immunity.Tag == Tag))
            return false;

        if (effects.Any(x => x is AudioEffect effect && effect.Tag == Tag))
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
