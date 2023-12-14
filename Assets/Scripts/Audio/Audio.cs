using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, InlineProperty, HideLabel]
public class Audio : IEnumerable, IEnumerable<AudioClip>, IReadOnlyList<AudioClip>
{
    private static IEnumerable<Type> PickerFilter 
        => _cachedFilter ??= ReflectionUtility.GetDerivedTypes<IAudioPicker>(AppDomain.CurrentDomain);

    private static IEnumerable<Type> _cachedFilter;

    public int Count => Clips.Count;
    public AudioClip this[int index] => Clips[index];

    public List<AudioClip> Clips;
    [SerializeReference, TypeFilter(nameof(PickerFilter)), Indent]
    public IAudioPicker Picker = new AudioSequence();

    public AudioClip Get() => Picker.Get(Clips);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<AudioClip> GetEnumerator()
    {
        for (int i = 0; i < Clips.Count; i++)
            yield return Get();
    }

}
