using Sirenix.OdinInspector;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioPickerShuffle : IAudioPicker
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

                    if (_shuffledIndices != null)
                        ArrayPool<int>.Shared.Return(_shuffledIndices);

                    _shuffledIndices = ArrayPool<int>.Shared.Rent(_count);

                    int i = 0;
                    foreach (int item in Enumerable.Range(0, _count).OrderBy(x => UnityEngine.Random.value))
                        _shuffledIndices[i++] = item;
                }

                return clips[_shuffledIndices[_currentIndex++]];
            case DistributionType.Weighted:
                float randomValue = UnityEngine.Random.value;
                float distributionValue = WeightDistribution.Evaluate(randomValue);
                int index = Mathf.RoundToInt(distributionValue * (clips.Count - 1));
                return clips[index];
            default:
                return clips[UnityEngine.Random.Range(0, clips.Count)];
        }
    }

    public void Reset()
    {
        ArrayPool<int>.Shared.Return(_shuffledIndices);
        _shuffledIndices = null;
        _currentIndex = 0;
        _count = 0;
    }
}
