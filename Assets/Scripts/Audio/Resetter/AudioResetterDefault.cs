using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AudioResetterDefault : IAudioResetter
{
    public bool Reset(IAudioPicker picker, IList<Playlist.Clip> clips) => false;
}
