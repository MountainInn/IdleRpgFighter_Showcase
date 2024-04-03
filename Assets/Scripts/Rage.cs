using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Rage", menuName = "SO/Abilities/Rage")]
public class Rage : Ability
{
    [SerializeField] [HideInInspector] List<Field> damageMultipliers;
    [SerializeField] [HideInInspector] List<Field> durations;

    Buff attackBuff = new();

    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        attackBuff
            .Subscribe(character.preAttack.AsObservable(),
                       (args, mult) => args.damage *= mult)
            .AddTo(abilityButton);
    }

    protected override void Use()
    {
        attackBuff.StartBuff(character);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Attack Buff");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);

        attackBuff.multiplier = damageMultipliers[level];
        attackBuff.duration = durations[level];
    }
}
