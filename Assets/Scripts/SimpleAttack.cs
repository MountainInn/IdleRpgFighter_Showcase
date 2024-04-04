using System;
using UnityEngine;
using UniRx;
using Zenject;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SimpleAttack", menuName = "SO/Abilities/SimpleAttack")]
public class SimpleAttack : Ability_Attack
{
    [SerializeField] [HideInInspector] List<Field> cooldowns;

    protected override void Use()
    {
        lastCreatedArgs = character.CreateDamage();
        lastCreatedArgs.animationTrigger = attackAnimationTrigger;

        character.PushAttack(lastCreatedArgs);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Attack");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        cooldown.Resize(cooldowns[level]);

        CostUp(level, price);
    }
}
