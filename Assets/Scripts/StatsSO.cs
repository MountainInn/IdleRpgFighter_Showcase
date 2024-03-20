using UnityEngine;

public class StatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float attackDamage;
    [SerializeField] public float attackTimer;
    [SerializeField] public float critChance;
    [SerializeField] public float critMult;

    public void Apply(Combatant combatant)
    {
        var stats = combatant.Stats = Instantiate(this);

        combatant.health.ResizeAndRefill(health);
        combatant.attackTimer.ResetToZero();
        combatant.attackTimer.Resize(attackTimer);
    }
}
