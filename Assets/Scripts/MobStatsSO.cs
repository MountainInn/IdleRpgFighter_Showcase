using UnityEngine;

[CreateAssetMenu(fileName = "MobStatsSO", menuName = "MobStatsSO")]
public class MobStatsSO : StatsSO
{
    [SerializeField] public CombatantTemplate template;

    [SerializeField] public DropList dropList;

    public void Apply(Mob mob)
    {
        mob.attackTimer.ResetToZero();
        mob.attackTimer.Resize(attackSpeed);
    }

    public void Multiply(float multiplier)
    {
        health *= multiplier;
        attackDamage *= multiplier;
    }
}
