using UnityEngine;

[CreateAssetMenu(fileName = "MobStats", menuName = "SO/Stats/Mob Stats")]
public class MobStatsSO : StatsSO
{
    [Space]
    [SerializeField] public CombatantTemplate template;
    [Space]
    [SerializeField] public DropList dropList;

    public void Apply(Mob mob)
    {
        mob.attackTimer.ResetToZero();
        mob.attackTimer.Resize(attackTimer);
    }

    public void Multiply(float multiplier)
    {
        health *= multiplier;
        attackDamage *= multiplier;
    }
}
