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
        public float damage;
        public int price;
    }

    float damage;

    protected override void Use()
    {
        lastCreatedArgs = character.CreateDamage();
        lastCreatedArgs.damage = damage;
        lastCreatedArgs.isPower = true;
        lastCreatedArgs.animationTrigger = attackAnimationTrigger;

        character.PushAttack(lastCreatedArgs);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Power Attack");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        damage = fields[level].damage;

        CostUp(level, price);
    }
}
