using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void Play(this AudioSource source, Playlist playlist)
    {
        source.clip = playlist.Get();
        source.Play();
    }
}
