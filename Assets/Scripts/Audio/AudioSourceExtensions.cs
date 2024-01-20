using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void Play(this AudioSource source, Playlist playlist)
    {
        var clip = playlist.Get();

        if (clip == null)
            return;

        source.clip = clip.Value.AudioClip;
        source.Play();
    }

    public static void PlayOneShot(this AudioSource source, Playlist playlist, float volumeScale = 1)
    {
        var clip = playlist.Get();

        if (clip == null)
            return;

        source.PlayOneShot(clip?.AudioClip, clip.Value.VolumeScale * volumeScale);
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

    public static async UniTask FadeIn(this AudioSource audioSource, float fadeDuration, CancellationToken cancellationToken)
    {
        float defaultVolume = audioSource.volume;

        float startVolume = 0.2f;
        audioSource.volume = 0;

        audioSource.Play();

        while (audioSource.volume < defaultVolume && !cancellationToken.IsCancellationRequested)
        {
            audioSource.volume += startVolume * (Time.deltaTime / fadeDuration);

            await UniTask.Yield(cancellationToken: cancellationToken);
        }

        audioSource.volume = defaultVolume;
    }

    public static async UniTask FadeOut(this AudioSource audioSource, float fadeDuration, CancellationToken cancellationToken)
    {
        float defaultVolume = audioSource.volume;

        while (audioSource.volume > 0 && !cancellationToken.IsCancellationRequested)
        {
            audioSource.volume -= defaultVolume * (Time.deltaTime / fadeDuration);

            await UniTask.Yield(cancellationToken: cancellationToken);
        }

        audioSource.Stop();

        audioSource.volume = defaultVolume;
    }
}
