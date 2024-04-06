using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random_Audio Clip settings", menuName = "SO/Audio/Random Clip")]
public class RandomAudioCueSO : AudioCueSOBase
{
    [SerializeField] List<AudioClipData> _audioClipDatas = new List<AudioClipData>() { new AudioClipData() { Volume = 1f } };

    public override AudioClipData GetAudioClip()
    {
        return _audioClipDatas.GetRandom();
    }
}


