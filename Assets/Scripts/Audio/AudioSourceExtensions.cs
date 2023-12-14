using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void Play(this AudioSource source, Audio audio)
    {
        source.clip = audio.Get();
        source.Play();
    }
}
