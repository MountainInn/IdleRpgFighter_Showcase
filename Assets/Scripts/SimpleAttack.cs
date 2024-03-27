using System;
using UnityEngine;
using UniRx;
using Zenject;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SimpleAttack", menuName = "SO/Abilities/SimpleAttack")]
public class SimpleAttack : Ability
{
    [SerializeField] List<Field> fields;

    [Serializable]
    struct Field
    {
        public float cooldown;
        public int price;
    }

    protected override void ConcreteSubscribe()
    {
        abilityButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                character.Attack();
                DrainEnergy();
                cooldown.ResetToZero();
            })
            .AddTo(character);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Attack");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        cooldown.Resize(fields[level].cooldown);

        price.cost.Value = fields[level].price;
    }
}
