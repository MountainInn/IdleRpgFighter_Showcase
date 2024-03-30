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
    [SerializeField] List<Field> fields;

    [Serializable]
    struct Field
    {
        public float multiplier;
        public float duration;
        public int price;
    }

    FortificationBuff buff = new();

    [Inject]
    public void Construct(Block block)
    {
        buff.block = block;
    }

    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        buff.Subscribe(character);
    }

    protected override void Use()
    {
        buff.StartBuff(abilityButton.gameObject);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Fortification");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;

        buff.multiplier = fields[level].multiplier;
        buff.duration = fields[level].duration;
    }

    public class FortificationBuff : Buff
    {
        public Block block;

        public override void Subscribe(Combatant combatant)
        {
        }

        protected override void ActivateBonus()
        {
            block.fortificationMult = multiplier;
            block.energyDrainMult = 0;
        }
        protected override void DeactivateBonus()
        {
            block.fortificationMult = 1;
            block.energyDrainMult = 1;
        }
    }
}
