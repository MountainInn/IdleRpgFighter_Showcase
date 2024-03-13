using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "SO/Stats/Character Stats")]
public class CharacterStatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float critChance;
    [SerializeField] public float critMult;

    public StatsSO ToStats()
    {
        return new StatsSO
        {
            attackDamage = 0,
            health = health,
            attackTimer = attackSpeed,
            critChance = critChance,
            critMult = critMult
        };
    }
}
