using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Playlist;

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
#if UNITY_EDITOR
    [ValueDropdown(nameof(GetPickerDropdown), ExpandAllMenuItems = true)]
#endif
    [SerializeReference, Indent]
    private IAudioPicker _picker = new AudioPickerSequence();
#if UNITY_EDITOR
    [ValueDropdown(nameof(GetResetterDropdown), ExpandAllMenuItems = true)]
#endif
    [SerializeReference, Indent, PropertySpace(spaceAfter: 8, spaceBefore: 0)]
    private IAudioResetter _resetter = new AudioResetterDefault();

    [Serializable]
    public struct Clip
    {
        public readonly bool IsEmpty => AudioClip == null;

#if UNITY_EDITOR
[InlineButton(nameof(ExpandButton), Icon = SdfIconType.CaretDownFill, Label = "")]
#endif
        [HorizontalGroup("Split", 0.8f), HideLabel]
        public AudioClip AudioClip;
        [HorizontalGroup("Split"), HideLabel]
        public float VolumeScale;
        [SerializeReference, HideLabel, ShowIf(nameof(_showSettings))]
        public IAudioClipSettings Settings;

        public readonly void Play(AudioSource source, float volumeScale = 1)
        {
            if (IsEmpty)
                return;

            source.clip = AudioClip;
            source.volume = AudioSourceExtensions.GetDefaultVolume(source) * VolumeScale * volumeScale;
            source.Play();

            float startFrame = 0;
            Settings?.Initialize(in this, AudioClip.length, out startFrame);
            source.time = startFrame;
        }

        public readonly void PlayOneShot(AudioSource source, float volumeScale = 1)
        {
            if (IsEmpty)
                return;

            source.PlayOneShot(AudioClip, VolumeScale * volumeScale);
        }

#if UNITY_EDITOR
        private bool _showSettings;

        private void ExpandButton()
            => _showSettings = !_showSettings;
#endif
    }

    public Playlist.Clip Get()
    {
        if (_clips.Count == 0)
            return default;

        if (_resetter.Reset(_picker, _clips))
            ResetPicker();

        return _picker.Get(_clips);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<AudioClip> GetEnumerator()
    {
        for (int i = 0; i < _clips.Count; i++)
            yield return Get().AudioClip;
    }

    public void ResetPicker()
        => Picker.Reset();

#if UNITY_EDITOR
    private static Clip AddClip()
        => new() 
            { 
                VolumeScale = 1,
            };

    private IEnumerable GetPickerDropdown()
        => PickerFilter.Select(type => new ValueDropdownItem(type.Name.Replace("AudioPicker", ""),
            _picker.GetType() == type ? _picker : Activator.CreateInstance(type)));
    private IEnumerable GetResetterDropdown()
    => ResetterFilter.Select(type => new ValueDropdownItem(type.Name.Replace("AudioResetter", ""), 
        _resetter.GetType() == type ? _resetter : Activator.CreateInstance(type)));
#endif
}
