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

    float duration;

    [HideInInspector] public BoolReactiveProperty enabled = new();
   
    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        enabled
            .WhereEqual(true)
            .Subscribe(_ =>
            {
                character
                    .onAttackPushed
                    .AsObservable()
                    .TakeUntil( enabled.WhereEqual(false) )
                    .Subscribe(args =>
                               args.animationTrigger = noTimeAttack_AnimationTrigger)
                    .AddTo(abilityButton);
            })
            .AddTo(abilityButton);
    }

    protected override void Use()
    {
        Observable
            .Timer(TimeSpan.FromSeconds(duration))
            .DoOnSubscribe(() => enabled.Value = true)
            .DoOnCancel(() => enabled.Value = false)
            .Subscribe()
            .AddTo(abilityButton);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("LightSpeedMode");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;

        duration = fields[level].duration;
    }
}

