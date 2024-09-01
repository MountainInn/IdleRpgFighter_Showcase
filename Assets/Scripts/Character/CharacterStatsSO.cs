using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "SO/Stats/Character Stats")]
public class CharacterStatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float critChance;
    [SerializeField] public float critMult;

    public StatsSO ToStats()
    {
        return new StatsSO
        {
            attackDamage = 0,
            health = health,
            attackTimer = 1,
            critChance = critChance,
            critMult = critMult
        };
    }
}
