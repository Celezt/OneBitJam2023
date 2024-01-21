using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AudioFramerDefault : IAudioFramer
{
    public readonly float Frame(in Playlist.Clip clip, float duration) => 0;   // Start time.
}
