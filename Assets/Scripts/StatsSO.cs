using UnityEngine;

[CreateAssetMenu(fileName = "StatsSO", menuName = "StatsSO")]
public class StatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float attackDamage;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float critChance;
    [SerializeField] public float critMult;

    public void Apply(Combatant combatant)
    {
        combatant.health.ResizeAndRefill(health);
    }
}
