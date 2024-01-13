using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, InlineProperty]
public class Playlist : IEnumerable, IEnumerable<AudioClip>, IReadOnlyList<AudioClip>
{
    public static IEnumerable<Type> PickerFilter 
        => _cachedPickers ??= ReflectionUtility.GetDerivedTypes<IAudioPicker>(AppDomain.CurrentDomain);
    public static IEnumerable<Type> ResetterFilter
        => _cachedResetters ??= ReflectionUtility.GetDerivedTypes<IAudioResetter>(AppDomain.CurrentDomain);

    private static IEnumerable<Type> _cachedPickers;
    private static IEnumerable<Type> _cachedResetters;

    public int Count => _clips.Count;
    public AudioClip this[int index] => _clips[index].AudioClip;

    public List<Clip> Clips => _clips;
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

    [SerializeField]
#if UNITY_EDITOR
    [ListDrawerSettings(CustomAddFunction = nameof(AddClip))]
#endif
    private List<Clip> _clips = new();
    [SerializeReference, TypeFilter(nameof(PickerFilter)), Indent]
    private IAudioPicker _picker = new AudioPickerSequence();
    [SerializeReference, TypeFilter(nameof(ResetterFilter)), Indent, PropertySpace(spaceAfter: 8, spaceBefore: 0)]
    private IAudioResetter _resetter = new AudioResetterDefault();

    [Serializable]
    public struct Clip
    {
        [HorizontalGroup("Split", 0.8f), HideLabel]
        public AudioClip AudioClip;
        [HorizontalGroup("Split"), HideLabel]
        public float VolumeScale;
    }

    public Playlist.Clip? Get()
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
            yield return Get()?.AudioClip;
    }

    public void ResetPicker()
        => Picker.Reset();

#if UNITY_EDITOR
    private Clip AddClip()
        => new Clip { VolumeScale = 1 };
#endif
}
