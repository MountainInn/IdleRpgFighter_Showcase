using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "AttackLevel", menuName = "SO/AttackLevel")]
public class AttackLevel : Talent
{
    [SerializeField] List<int> attackOnLevel;
    [SerializeField] List<int> priceOnLevel;

    [Inject] Character character;

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l =>
            {
                int currentDamage = attackOnLevel[l];
                string nextDamage;

                if (attackOnLevel.Count > l + 1)
                    nextDamage = $"{attackOnLevel[l+1]}";
                else
                    nextDamage = "MAX";

                return $"Character damage {currentDamage} -> {nextDamage}";
            });
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = priceOnLevel[level];
        character.Stats.attackDamage = attackOnLevel[level];
    }
}
