using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioPickerShuffle;

public class AudioFramerRandom : IAudioFramer
{
    public DistributionType Distribution = DistributionType.Random;
    [ShowIf(nameof(Distribution), DistributionType.Weighted)]
    public AnimationCurve WeightDistribution = AnimationCurve.Linear(0, 0, 1, 1);

    public enum DistributionType
    {
        Random,
        Weighted,
    }

    public float Frame(in Playlist.Clip clip, float duration)
    {
        switch (Distribution)
        {
            case DistributionType.Weighted:
                float randomValue = UnityEngine.Random.value;
                float distributionValue = Mathf.Clamp01(WeightDistribution.Evaluate(randomValue));
                return distributionValue * duration;
            default:
                return UnityEngine.Random.value * duration;
        }
    }
}
