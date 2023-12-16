using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioResetterDefault : IAudioResetter
{
    public bool Reset(IAudioPicker picker, IList<AudioClip> clips) => false;
}
