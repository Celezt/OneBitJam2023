using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class PlaylistBehaviour : MonoBehaviour
{
    public Playlist Playlist => _playlist;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField, Space(4)]
    private Playlist _playlist;

    public void Play()
    {
        _audioSource.Play(_playlist);
    }

    public void PlayOneShot()
    {
        _audioSource.PlayOneShot(_playlist);
    }
}