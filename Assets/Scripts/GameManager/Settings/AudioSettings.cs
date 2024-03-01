using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[GlobalSettings]
public class AudioSettings : ISettings
{
    [SerializeField] 
    private AudioMixer _audioMixer;
    [SerializeField] 
    private string[] _volumeParameters;

    public void LoadAudioSettings()
    {
        foreach (string volumeParameter in _volumeParameters)
            Load(volumeParameter);
    }

    public void SaveAudioSettings()
    {
        foreach (string volumeParameter in _volumeParameters)
            Save(volumeParameter);
    }

    public void GameStart(IEnumerable<ISettings> settings)
    {
        LoadAudioSettings();
    }

    public void GameExit(IEnumerable<ISettings> settings)
    {
        SaveAudioSettings();
    }

    private void Save(string volumeParameter)
    {
        if (!_audioMixer)
            return;

        if (!_audioMixer.GetFloat(volumeParameter, out float volume))
            return;

        PlayerPrefs.SetFloat(volumeParameter, volume);
    }

    private void Load(string volumeParameter)
    {
        if (!_audioMixer)
            return;

        if (!PlayerPrefs.HasKey(volumeParameter))
            return;

        _audioMixer.SetFloat(volumeParameter, PlayerPrefs.GetFloat(volumeParameter));
    }
}
