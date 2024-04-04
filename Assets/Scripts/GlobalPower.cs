using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "GlobalPower", menuName = "SO/Stats/GlobalPower")]
public class GlobalPower : Stat
{
    [SerializeField] [HideInInspector] List<Field> globalPowers;

    [Inject] Character character;

    public override int CurrentValue => globalPowers[Level];

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l => GetFieldDescriptions(l, ("Global Power", globalPowers)));
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
    }
}
