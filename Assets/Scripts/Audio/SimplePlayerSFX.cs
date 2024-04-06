using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SimplePlayerSFX : MonoBehaviour
{
    AudioManager _audioManager;

    [Inject]
    private void Constract(AudioManager manager)
    {
        _audioManager = manager;
    }
    public void PlaySfx(IAudioClipProvider clip)
    {
        _audioManager.PlaySFX(clip.GetAudioClip());
    }
}
