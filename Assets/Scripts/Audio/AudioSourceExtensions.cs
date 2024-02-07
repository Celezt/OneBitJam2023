using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class AudioSourceExtensions
{
    private readonly static Dictionary<AudioSource, float> _defaultVolumes = new();

    public static UniTask PlayAsync(this AudioSource source, Playlist playlist, CancellationToken cancellationToken = default, PlayerLoopTiming timing = PlayerLoopTiming.Update, float volumeScale = 1)
    {
        var clip = playlist.Get();

        if (clip.IsEmpty)
            return UniTask.CompletedTask;

        clip.Play(source, volumeScale);

        bool WaitUntil()
        {
            if (source.clip != clip.AudioClip)
                return true;

            if (!source.isPlaying)
                return true;

            return false;
        }

        return UniTask.WaitUntil(WaitUntil, cancellationToken: cancellationToken, timing: timing);
    }

    public static Playlist.Clip Play(this AudioSource source, Playlist playlist, float volumeScale = 1)
        => playlist.Get().Play(source, volumeScale);

    public static Playlist.Clip PlayOneShot(this AudioSource source, Playlist playlist, float volumeScale = 1)
        => playlist.Get().PlayOneShot(source, volumeScale);

    public static UniTask CrossFade(this AudioSource audioSource, Playlist playlist, float fadeDuration, CancellationToken cancellationToken)
    => CrossFade(audioSource, playlist, fadeDuration, fadeDuration, cancellationToken);
    public static async UniTask CrossFade(this AudioSource audioSource, Playlist playlist, float fadeInDuration, float fadeOutDuration, CancellationToken cancellationToken)
    {
        await FadeOut(audioSource, fadeOutDuration, cancellationToken);

        await FadeIn(audioSource, playlist, fadeInDuration, cancellationToken);
    }

    public static UniTask CrossFade(this AudioSource audioSource, AudioClip nextAudioClip, float fadeDuration, CancellationToken cancellationToken)
        => CrossFade(audioSource, nextAudioClip, fadeDuration, fadeDuration, cancellationToken);
    public static async UniTask CrossFade(this AudioSource audioSource, AudioClip nextAudioClip, float fadeInDuration, float fadeOutDuration, CancellationToken cancellationToken)
    {
        await FadeOut(audioSource, fadeOutDuration, cancellationToken);

        audioSource.clip = nextAudioClip;

        await FadeIn(audioSource, fadeInDuration, cancellationToken);
    }

    public static UniTask CrossFade(this AudioSource audioSource, AudioSource nextAudioSource, float fadeDuration, CancellationToken cancellationToken)
        => CrossFade(audioSource, nextAudioSource, fadeDuration, fadeDuration, cancellationToken);
    public static async UniTask CrossFade(this AudioSource audioSource, AudioSource nextAudioSource, float fadeInDuration, float fadeOutDuration, CancellationToken cancellationToken)
    {
        await FadeOut(audioSource, fadeOutDuration, cancellationToken);
        await FadeIn(nextAudioSource, fadeInDuration, cancellationToken);
    }

    public static async UniTask FadeIn(this AudioSource audioSource, Playlist playlist, float fadeDuration, CancellationToken cancellationToken)
    {
        float defaultVolume = audioSource.volume;

        float startVolume = 0.2f;
        audioSource.volume = 0;

        audioSource.Play(playlist);

        try
        {
            while (audioSource.volume < defaultVolume && !cancellationToken.IsCancellationRequested)
            {
                audioSource.volume += startVolume * (Time.deltaTime / fadeDuration);

                await UniTask.Yield(cancellationToken: cancellationToken);
            }
        }
        catch { }

        audioSource.volume = defaultVolume;
    }

    public static async UniTask FadeIn(this AudioSource audioSource, float fadeDuration, CancellationToken cancellationToken)
    {
        float defaultVolume = audioSource.volume;

        float startVolume = 0.2f;
        audioSource.volume = 0;

        audioSource.Play();

        try
        {
            while (audioSource.volume < defaultVolume && !cancellationToken.IsCancellationRequested)
            {
                audioSource.volume += startVolume * (Time.deltaTime / fadeDuration);

                await UniTask.Yield(cancellationToken: cancellationToken);
            }
        }
        catch { }

        audioSource.volume = defaultVolume;
    }

    public static async UniTask FadeOut(this AudioSource audioSource, float fadeDuration, CancellationToken cancellationToken)
    {
        if (!audioSource.isPlaying)
            return;

        float defaultVolume = audioSource.volume;

        try
        {
            while (audioSource.volume > 0 && !cancellationToken.IsCancellationRequested)
            {
                audioSource.volume -= defaultVolume * (Time.deltaTime / fadeDuration);

                await UniTask.Yield(cancellationToken: cancellationToken);
            }
        }
        catch { }

        audioSource.Stop();

        audioSource.volume = defaultVolume;
    }

    public static float SetVolume(this AudioSource source, float volume)
    {
        float previousVolume = GetDefaultVolume(source);

        if (previousVolume != volume)
            _defaultVolumes[source] = volume;

        source.volume = volume;
        return volume;
    }

    public static float SetVolumeScale(this AudioSource source, float volumeScale)
        => source.volume = GetDefaultVolume(source) * volumeScale;

    public static float GetDefaultVolume(AudioSource source)
    {
        if (!_defaultVolumes.TryGetValue(source, out float defaultVolume))  // Add default volume.
        {
            _defaultVolumes[source] = defaultVolume = source.volume;
            OnDestroyAsync(source).Forget();
        }

        static async UniTaskVoid OnDestroyAsync(AudioSource source)
        {
            await source.OnDestroyAsync();

            _defaultVolumes.Remove(source);
        }

        return defaultVolume;
    }
}
