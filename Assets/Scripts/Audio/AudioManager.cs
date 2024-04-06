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
    public void PlaySFX(AudioCueSO audioCue)
    {
        if (audioCue == null || audioCue.Audio == null) return;
        _source.PlayOneShot(audioCue.Audio,audioCue.Volume);
    }
    public void PlayMusic(AudioCueSO audioCue)
    {
        if (audioCue == null || audioCue.Audio == null) return;

        if (_playingName == audioCue.name)
            return;

        _playingName = audioCue.name;

        _source.clip = audioCue.Audio;
        _source.volume = audioCue.Volume;
        _source.loop = audioCue.Loop ;
        _source.Play();
    }

}
