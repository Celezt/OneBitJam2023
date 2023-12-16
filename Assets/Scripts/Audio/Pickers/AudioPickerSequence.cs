using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPickerSequence : IAudioPicker
{
    private int _currentIndex;

    public AudioClip Get(IList<AudioClip> clips)
        => clips[_currentIndex++ % clips.Count];

    public void Reset()
    {
        _currentIndex = 0;
    }
}
