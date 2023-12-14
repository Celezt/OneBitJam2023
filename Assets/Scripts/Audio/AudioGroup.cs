using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class AudioGroup : MonoBehaviour
{
    public float Volume
    {
        get => _volume;
        set => Mathf.Clamp01(value);
    }

    [SerializeField, Range(0, 1), Space(8)]
    private float _volume = 1.0f;

    private readonly List<AudioSource> _audioSources = new();

    private void Start()
    {
        foreach (AudioSource source in GetComponentsInChildren<AudioSource>())
            _audioSources.Add(source);
    }
}
