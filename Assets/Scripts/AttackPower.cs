using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using System.Linq;

[CreateAssetMenu(fileName = "AttackPower", menuName = "SO/Stats/AttackPower")]
public class AttackPower : Stat
{
    [SerializeField] [HideInInspector] List<Field> attackPower = new();

    [Inject] GlobalPower globalPower;
    [Inject] Character character;

    public override int CurrentValue => attackPower[Level];

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level.current
            .Select(l =>
                    GetFieldDescriptions((int)l,
                                         ("Attack Power", attackPower)
                    ));
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
        character.Stats.attackDamage = CurrentValue * globalPower.CurrentValue;
    }
}
