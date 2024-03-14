using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Linq;

public class Block : Ability
{
    [SerializeField] float parryTimeWindow;
    [SerializeField] float damageReductionMultiplier;
    [Space]
    [SerializeField] bool canCounterAttack;
    [Space]
    [SerializeField] float counterAttackDamageBonus;
    [SerializeField] float bonusDuration;

    float activeBonus = 1f;

    [Inject] BlockVfx blockVfx;
    [Inject] AttackBonusVfx attackBonusVfx;
    [Inject]
    public void Construct(Character character)
    {
        character.preAttack.AddListener( ApplyCounterAttackBonus );

        abilityButton
            .OnPointerDownAsObservable()
            .Subscribe(_ =>
            {
                SubscribeBlock(character);
                SubscribeParry(character);
            })
            .AddTo(character);
    }

    void ApplyCounterAttackBonus(DamageArgs hit)
    {
        hit.damage *= activeBonus;
    }

    void SubscribeBlock(Character character)
    {
        character.preTakeDamage.AsObservable()
            .TakeUntil(abilityButton.OnPointerUpAsObservable())
            .DoOnSubscribe(() =>
            {
                blockVfx.ShowBlock();
            })
            .Do(hit =>
            {
                hit.damage *= damageReductionMultiplier;
            })
            .DoOnCompleted(() =>
            {
                blockVfx.HideBlock();
                cooldown.ResetToZero();
            })
            .Subscribe()
            .AddTo(character);
    }

    void SubscribeParry(Character character)
    {
        character.preTakeDamage.AsObservable()
            .Take(1)
            .Take(TimeSpan.FromSeconds(parryTimeWindow))
            .DoOnSubscribe(() =>
            {
                blockVfx.ShowParry();
            })
            .Do(hit =>
            {
                if (hit != null && canCounterAttack)
                {
                    blockVfx.ShowCounterAttack();
                    SubscribeCounterAttackBonus(character);
                }
            })
            .DoOnCompleted(() =>
            {
                blockVfx.HideParry();
            })
            .Subscribe()
            .AddTo(character);
    }

    void SubscribeCounterAttackBonus(Character character)
    {
        Observable
            .Timer(TimeSpan.FromSeconds(bonusDuration))
            .DoOnSubscribe( ActivateBonus )
            .DoOnCompleted( DeactivateBonus )
            .Subscribe()
            .AddTo(character);
    }

    void ActivateBonus()
    {
        activeBonus = counterAttackDamageBonus;
        attackBonusVfx.ShowBonus();
    }
    void DeactivateBonus()
    {
        activeBonus = 1f;
        attackBonusVfx.HideBonus();
    }

    public override IObservable<string> ObserveDescription()
    {
        return
            Observable.Return("*BLANK*");
    }

    protected override void OnLevelUp(int level, Price price)
    {
    }
}
