using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource _source;
    string _playingName;

    private void OnValidate()
    {
        _source = _source?? GetComponent<AudioSource>();
    }
    public void PlaySFX(AudioClipData audioData)
    {
        if (audioData.Audio == null) return;
        _source.PlayOneShot(audioData.Audio, audioData.Volume);
    }
    public void PlayMusic(AudioClipData audioData)
    {
        if (audioData.Audio == null) return;

        if (_playingName == audioData.Audio.name)
            return;

        _playingName = audioData.Audio.name;

        _source.clip = audioData.Audio;
        _source.volume = audioData.Volume;
        _source.loop = audioData.Loop ;
        _source.Play();
    }

}
