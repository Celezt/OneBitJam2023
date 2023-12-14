using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AudioShuffle : IAudioPicker
{
    public DistributionType Distribution = DistributionType.Random;
    [ShowIf(nameof(Distribution), DistributionType.Weighted)]
    public AnimationCurve WeightDistribution = AnimationCurve.Linear(0, 0, 1, 1);

    private int[] _shuffledIndices;
    private int _currentIndex;
    private int _count;

    public enum DistributionType
    {
        Random,
        Unique,
        Weighted,
    }

    public AudioClip Get(IList<AudioClip> clips)
    {
        switch (Distribution)
        {
            case DistributionType.Unique:
                if (_shuffledIndices == null || _currentIndex >= _count || _count > clips.Count)
                {
                    _currentIndex = 0;
                    _count = clips.Count;
                    _shuffledIndices = Enumerable.Range(0, clips.Count).OrderBy(x => (int)(UnityEngine.Random.value * 100)).ToArray();
                }

                return clips[_shuffledIndices[_count++]];
            case DistributionType.Weighted:
                float randomValue = UnityEngine.Random.value;
                float distributionValue = WeightDistribution.Evaluate(randomValue);
                int index = (int)(distributionValue * clips.Count);
                return clips[index];
            default:
                return clips[UnityEngine.Random.Range(0, clips.Count)];
        }
    }
}
