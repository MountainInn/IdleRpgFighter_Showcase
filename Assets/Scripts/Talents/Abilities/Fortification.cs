using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Fortification", menuName = "SO/Abilities/Fortification")]
public class Fortification : Ability
{
    [SerializeField] [HideInInspector] List<Field> blockMultipliers;
    [SerializeField] [HideInInspector] List<Field> durations;

    Buff buff = new();

    [Inject] Block block;

    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        buff
            .Subscribe(toggle =>
            {
                if (toggle)
                {
                    block.fortificationMult = buff.multiplier;
                    block.energyDrainMult = 0;
                }
                else
                {
                    block.fortificationMult = 1;
                    block.energyDrainMult = 1;
                }
            })
            .AddTo(abilityButton);
    }

    protected override void Use()
    {
        buff.StartBuff(abilityButton);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Fortification");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);

        buff.multiplier = blockMultipliers[level];
        buff.duration = durations[level];
    }
}
