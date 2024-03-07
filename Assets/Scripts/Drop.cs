using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Drop", menuName = "SO/Drop")]
public class Drop : ScriptableObject
{
    [Range(0, 1)] public float chance;
    public Price currency;
    public GameObject prefabDropableGameObject;
}
