using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "EnergyLevel", menuName = "SO/Talents/EnergyLevel")]
public class EnergyLevel : Talent, ITickable
{
    [SerializeField] List<Field> fields;

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
            {
                int currentMaximum = fields[l].maximum;
                string nextMaximum;

                if (fields.Count > l + 1)
                    nextMaximum = $"{fields[l+1].maximum}";
                else
                    nextMaximum = "MAX";

                return $"Character energy max + regen {currentMaximum} -> {nextMaximum}";
            });
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;
        character.energy.ResizeAndRefill(fields[level].maximum);
    }

    [Serializable]
    struct Field
    {
        public int maximum;
        public float regenPerSecond;
        public int price;
    }

    public void Tick()
    {
        if (!character.energy.IsFull)
        {
            float amount =
                fields[buyableLevel.ware.level.Value].regenPerSecond
                * Time.deltaTime;

            character.energy.Add(amount);
        }
    }
}
