using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "LightSpeedMode", menuName = "SO/Abilities/LightSpeedMode")]
public class LightSpeedMode : Ability
{
    [SerializeField] public string noTimeAttack_AnimationTag = "no-time-attack-tag";
    [SerializeField] public string noTimeAttack_AnimationTrigger = "no-time-attack";

    [SerializeField] List<Field> fields;

    [Serializable]
    struct Field
    {
        public float duration;
        public int price;
    }

    public BoolReactiveProperty enabled => buff.enabled;

    Buff buff = new();

    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        buff
            .Subscribe(character.onAttackPushed.AsObservable(),
                       (args, _mult) =>
                       args.animationTrigger = noTimeAttack_AnimationTrigger)
            .AddTo(abilityButton);
    }

    protected override void Use()
    {
        buff.StartBuff(abilityButton);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("LightSpeedMode");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);

        buff.duration = fields[level].duration;
    }
}

