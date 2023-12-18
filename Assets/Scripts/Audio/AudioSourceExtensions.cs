using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void Play(this AudioSource source, Playlist playlist, bool ignoreNull = true)
    {
        var clip = playlist.Get();

        if (ignoreNull && clip == null)
            return;

        source.clip = playlist.Get();
        source.Play();
    }
}
