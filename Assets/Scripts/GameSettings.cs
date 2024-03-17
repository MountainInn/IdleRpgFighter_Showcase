using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "SO/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Loot Manager")]
    public float intervalBetweenDrops;
}
