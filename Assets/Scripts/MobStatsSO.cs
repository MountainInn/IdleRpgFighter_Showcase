using UnityEngine;

[CreateAssetMenu(fileName = "MobStatsSO", menuName = "MobStatsSO")]
public class MobStatsSO : StatsSO
{
    [SerializeField] public CombatantTemplate template;


    public void Multiply(float multiplier)
    {
        health *= multiplier;
        attackDamage *= multiplier;
    }
}
