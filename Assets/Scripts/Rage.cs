using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Rage", menuName = "SO/Abilities/Rage")]
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

    AttackBuff attackBuff = new();


    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        attackBuff.Subscribe(character);
    }

    protected override void Use()
    {
        attackBuff.StartBuff(character.gameObject);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Attack Buff");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;

        attackBuff.multiplier = fields[level].damageMultiplier;
        attackBuff.duration = fields[level].duration;
    }
}
