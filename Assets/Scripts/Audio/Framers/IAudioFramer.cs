using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioFramer
{
    public float Frame(in Playlist.Clip clip, float duration);
}
