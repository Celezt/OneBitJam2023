using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioResetter
{
    public bool Reset(IAudioPicker picker, IList<AudioClip> clips);
}
