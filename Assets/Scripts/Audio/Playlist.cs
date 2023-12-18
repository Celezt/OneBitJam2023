using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, InlineProperty, HideLabel]
public class Playlist : IEnumerable, IEnumerable<AudioClip>, IReadOnlyList<AudioClip>
{
    public static IEnumerable<Type> PickerFilter 
        => _cachedPickers ??= ReflectionUtility.GetDerivedTypes<IAudioPicker>(AppDomain.CurrentDomain);
    public static IEnumerable<Type> ResetterFilter
        => _cachedResetters ??= ReflectionUtility.GetDerivedTypes<IAudioResetter>(AppDomain.CurrentDomain);

    private static IEnumerable<Type> _cachedPickers;
    private static IEnumerable<Type> _cachedResetters;

    public int Count => _clips.Count;
    public AudioClip this[int index] => _clips[index];

    public List<AudioClip> Clips => _clips;
    public IAudioPicker Picker
    {
        get => _picker;
        set => _picker = value ?? new AudioPickerSequence();
    }
    public IAudioResetter Resetter
    {
        get => _resetter;
        set => _resetter = value ?? new AudioResetterDefault();
    }

    [Title("Playlist", horizontalLine: false, bold: false), PropertySpace(SpaceBefore = 8)]
    [SerializeField]
    private List<AudioClip> _clips = new();
    [SerializeReference, TypeFilter(nameof(PickerFilter)), Indent]
    private IAudioPicker _picker = new AudioPickerSequence();
    [SerializeReference, TypeFilter(nameof(ResetterFilter)), Indent, PropertySpace(spaceAfter: 8, spaceBefore: 0)]
    private IAudioResetter _resetter = new AudioResetterDefault();

    public AudioClip Get()
    {
        if (_clips.Count == 0)
            return null;

        if (_resetter.Reset(_picker, _clips))
            ResetPicker();

        return _picker.Get(_clips);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<AudioClip> GetEnumerator()
    {
        for (int i = 0; i < _clips.Count; i++)
            yield return Get();
    }

    public void ResetPicker()
        => Picker.Reset();
}
