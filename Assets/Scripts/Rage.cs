using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class Rage : Ability
{
    [SerializeField] List<Field> fields;

    [Serializable]
    struct Field
    {
        public float damageMultiplier;
        public float duration;
        public int price;
    }

    float damageMultiplier;
    float duration;
    AttackBuff attackBuff;


    protected override void Use()
    {
        attackBuff.StartBuff(character.gameObject);
    }

    protected override void ConcreteSubscribe()
    {
        attackBuff = new () { duration = duration,
                              multiplier = damageMultiplier };

        attackBuff.Subscribe(character);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Attack Buff");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;
        damageMultiplier = fields[level].damageMultiplier;
        duration = fields[level].duration;
    }
}
