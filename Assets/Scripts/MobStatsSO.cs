using UnityEngine;

[CreateAssetMenu(fileName = "MobStats", menuName = "SO/Stats/Mob Stats")]
public class MobStatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float attackDamage;
    [SerializeField] public float attackTimer;
    [Space]
    [SerializeField] public CombatantTemplate template;
    [Space]
    [SerializeField] public DropList dropList;

    public StatsSO ToStats()
    {
        return new StatsSO
        {
            health = health,
            attackDamage = attackDamage,
            attackTimer = attackTimer,
            critChance = 0,
            critMult = 1,
        };
    }

    public void Apply(Mob mob)
    {
        this . ToStats() . Apply(mob);

        mob.dropList = this.dropList;

        template.ApplyTemplate(mob.gameObject);
    }

    public void Apply(Ally ally)
    {
        this . ToStats() . Apply(ally);

        template.ApplyTemplate(ally.gameObject);
    }

    public void Multiply(float multiplier)
    {
        health *= multiplier;
        attackDamage *= multiplier;
    }
}
