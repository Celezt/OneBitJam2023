using System.Collections;
using System.Collections.Generic;
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
}
