using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "SO/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Loot Manager")]
    public float intervalBetweenDrops;
    public int maxParticleCount;
    [Header("Save System")]
    public double autoSaveInterval;
    [Header("Gulag")]
    public float gulagDuration;
}
