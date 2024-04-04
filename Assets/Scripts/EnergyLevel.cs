using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "EnergyLevel", menuName = "SO/Talents/EnergyLevel")]
public class EnergyLevel : Stat, ITickable
{
    [SerializeField] [HideInInspector] List<Field> maximumEnergy;
    [SerializeField] [HideInInspector] List<Field> regenPerSecond;

    [Inject] Character character;
    [Inject]
    public void Construct()
    {
        character.AddTickable(this);
    }

    public override IObservable<string> ObserveDescription()
    {
        return
            this.buyableLevel.ware.level
            .Select(l =>
                    GetFieldDescriptions(l,
                                         ("Maximum Energy", maximumEnergy),
                                         ("Regen", regenPerSecond)
                    ));
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
        character.energy.ResizeAndRefill(maximumEnergy[level]);
    }

    public void Tick()
    {
        if (!character.energy.IsFull)
        {
            float amount =
                regenPerSecond[buyableLevel.ware.level.Value]
                * Time.deltaTime;

            character.energy.Add(amount);
        }
    }
}
