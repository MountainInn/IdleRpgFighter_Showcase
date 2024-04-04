using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "MiningPower", menuName = "SO/Talents/MiningPower")]
public class MiningPower : Stat
{
    [SerializeField] [HideInInspector] List<Field> miningPowers;

    [Inject] Character character;

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l =>
                    GetFieldDescriptions(l,
                                         ("Mining Power", miningPowers)
                    ));
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
        character.Stats.attackDamage = miningPowers[level];
    }
}
