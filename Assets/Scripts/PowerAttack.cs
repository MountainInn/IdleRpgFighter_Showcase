using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PowerAttack", menuName = "SO/Abilities/PowerAttack")]
public class PowerAttack : Ability_Attack
{
    [SerializeField] List<Field> fields;

    [Serializable]
    struct Field
    {
        public float damageMultiplier;
        public int price;
    }

    float damageMultiplier;

    protected override void Use()
    {
        lastCreatedArgs = character.CreateDamage();
        lastCreatedArgs.damage *= damageMultiplier;
        lastCreatedArgs.isPower = true;

        cooldown.ResetToZero();

        character.PushAttack(this);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Power Attack");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        damageMultiplier = fields[level].damageMultiplier;

        price.cost.Value = fields[level].price;
    }
}
