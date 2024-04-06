using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName ="Audio Clip settings", menuName ="SO/Audio/Clip")]
public class AudioCueSO : ScriptableObject,IAudioClipProvider
{
    [SerializeField] AudioClipData _audioClipData = new AudioClipData() { Volume = 1f};

    public AudioClipData GetAudioClip()
    {
        return _audioClipData;
    }
}
[System.Serializable]
public struct AudioClipData
{
    public AudioClip Audio;
    public bool Loop;
    [Range(0, 1)] public float Volume;
}