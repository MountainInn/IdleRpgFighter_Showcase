using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName ="Audio Clip settings", menuName ="SO/Audio/Clip")]
public class AudioCueSO : ScriptableObject
{
    public AudioClip Audio;
    public bool Loop;
    [Range(0,1)]public float Volume =1;

}
