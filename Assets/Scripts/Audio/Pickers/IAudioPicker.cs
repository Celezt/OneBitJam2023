using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioPicker
{
    public AudioClip Get(IList<AudioClip> clips);
}
