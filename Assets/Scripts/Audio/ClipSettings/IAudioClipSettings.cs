using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioClipSettings
{
    public void Initialize(in Playlist.Clip clip, float duration, out float startFrame);
}
