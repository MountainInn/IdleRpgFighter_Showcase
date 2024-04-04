using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using System.Linq;

[CreateAssetMenu(fileName = "AttackLevel", menuName = "SO/Talents/AttackLevel")]
public class AttackLevel : Stat
{
    [SerializeField] [HideInInspector] List<Field> attackPower = new();

    [Inject] Character character;

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l =>
                    GetFieldDescriptions(l,
                                         ("Attack Power", attackPower)
                    ));
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
        character.Stats.attackDamage = attackPower[level];
    }
}
