using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipSettingsDefault : IAudioClipSettings
{
    public StartFrameType StartFrame = StartFrameType.Manual;
    [ShowIf(nameof(StartFrame), StartFrameType.Manual), MinValue(0)]
    public float StartTime = 0;
    [ShowIf(nameof(StartFrame), StartFrameType.Weighted)]
    public AnimationCurve WeightDistribution = AnimationCurve.Linear(0, 0, 1, 1);

    public enum StartFrameType
    {
        Manual,
        Random,
        Weighted,
    }

    public void Initialize(in Playlist.Clip clip, float duration, out float startFrame)
    {
        switch (StartFrame)
        {
            case StartFrameType.Random:
                startFrame = UnityEngine.Random.value * duration;
                break;
            case StartFrameType.Weighted:
                float randomValue = UnityEngine.Random.value;
                float distributionValue = Mathf.Clamp01(WeightDistribution.Evaluate(randomValue));
                startFrame = distributionValue * duration;
                break;
            default:
                startFrame = StartTime;
                break;
        }
    }
}
