using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "GlobalPower", menuName = "SO/Talents/GlobalPower")]
public class GlobalPower : Stat
{
    [SerializeField] [HideInInspector] List<Field> globalPowers;

    [Inject] Character character;

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l => GetFieldDescriptions(l, ("Global Power", globalPowers)));
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
        character.Stats.attackDamage = globalPowers[level];
    }
}
