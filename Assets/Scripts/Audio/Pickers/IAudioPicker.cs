using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioPicker : IReset
{
    public AudioClip Get(IList<AudioClip> clips);
}
