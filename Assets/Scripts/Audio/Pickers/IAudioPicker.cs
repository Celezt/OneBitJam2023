using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioPicker : IReset
{
    public Playlist.Clip Get(IList<Playlist.Clip> clips);
}
