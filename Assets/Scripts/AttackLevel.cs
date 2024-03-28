using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "AttackLevel", menuName = "SO/Talents/AttackLevel")]
public class AttackLevel : Talent
{
    [SerializeField] List<Field> fields;
    [SerializeField] public StatList attackDamage;

    [Inject] Character character;

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l =>
            {
                int currentDamage = fields[l].attack;
                string nextDamage;

                if (fields.Count > l + 1)
                    nextDamage = $"{fields[l+1].attack}";
                else
                    nextDamage = "MAX";

                return $"Character damage {currentDamage} -> {nextDamage}";
            });
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;
        character.Stats.attackDamage = fields[level].attack;
    }

    [Serializable]
    struct Field
    {
        public int attack, price;
    }
}
