using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AudioEffectorProperty : IEffectorProperty
{
    public GameObject AudioSourcePrefab;
    public InclusionType Inclusion = InclusionType.Include;
    [LabelText("@Inclusion == InclusionType.Include ? \"Include Tags\" : \"Exclude Tags\""), Indent]
    public List<string> Tags = new List<string>();
    public Playlist Playlist;

    private AudioPool _audioPool;
    private CancellationToken _cancellationToken;
    private float _timeSincePlay;

    public enum TagType
    {
        Include,
        Exclude,
    }

    public void OnEnable(IEffector effector)
    {
        _audioPool = new AudioPool(AudioSourcePrefab);
        effector.OnEffectAddedCallback += OnEffectAdded;

        _cancellationToken = effector.GameObject.GetCancellationTokenOnDestroy();
    }

    public void OnDisable(IEffector effector)
    {
        effector.OnEffectAddedCallback -= OnEffectAdded;
    }

    private void OnEffectAdded(IEffector effector, IEffect effect, GameObject sender)
    {
        float time = Time.time;

        if (_timeSincePlay == time)
            return;

        _timeSincePlay = time;

        if (Tags.Count > 0)
        {
            if (!(effect is IEffectTag effectTag))
                return;

            switch (Inclusion)
            {
                case InclusionType.Include when !Tags.Contains(effectTag.Tag):
                case InclusionType.Exclude when Tags.Contains(effectTag.Tag):
                    return;
            }
        }

        OnPlayAsync(_cancellationToken, sender).Forget();
    }

    private async UniTaskVoid OnPlayAsync(CancellationToken cancellationToken, GameObject sender)
    {
        var audioSource = _audioPool.Get();

        bool defaultLoop = audioSource.loop;
        audioSource.loop = false;

        var audioSourceTransform = audioSource.gameObject.transform;
        var senderTransform = sender.transform;

        audioSourceTransform.position = senderTransform.position;

        try
        {
            await audioSource.PlayAsync(Playlist, cancellationToken);
        }
        catch { }

        audioSource.loop = defaultLoop;

        _audioPool.Release(audioSource);
    }
}
