using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPickerSequence : IAudioPicker
{
    private int _currentIndex;

    public Playlist.Clip Get(IList<Playlist.Clip> clips)
        => clips[_currentIndex++ % clips.Count];

    public void Reset()
    {
        _currentIndex = 0;
    }
}
