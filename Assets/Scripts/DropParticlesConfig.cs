using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropParticlesConfig", menuName = "SO/DropParticlesConfig")]
public class DropParticlesConfig : ScriptableObject
{
    public List<Field> fields;

    [System.SerializableAttribute]
    public class Field
    {
        [SerializeField] public int goldAmount;
        [SerializeField] public LootParticles lootParticles;

    }
}
