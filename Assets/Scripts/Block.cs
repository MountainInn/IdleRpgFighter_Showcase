using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Linq;

[CreateAssetMenu(fileName = "Block", menuName = "SO/Abilities/Block")]
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
    bool isHoldingBlock;

    BlockVfx blockVfx;
    AttackBonusVfx attackBonusVfx;

    public void Subscribe(BlockVfx blockVfx, AttackBonusVfx attackBonusVfx)
    {
        this.blockVfx = blockVfx;
        this.attackBonusVfx = attackBonusVfx;
    }

    protected override void ConcreteSubscribe()
    {
        character.preAttack.AddListener( ApplyCounterAttackBonus );

        abilityButton
            .OnPointerDownAsObservable()
            .Subscribe(_ =>
            {
                if (isReadyToUse.Value)
                {
                    SubscribeBlock(character);
                    SubscribeParry(character);
                }
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
            .TakeUntil( ObserveHaveEnoughEnergy(Time.deltaTime).WhereEqual(false) )
            .TakeUntil( abilityButton.OnPointerUpAsObservable() )
            .DoOnSubscribe(() =>
            {
                blockVfx.ShowBlock();
                isHoldingBlock = true;
            })
            .Do(hit =>
            {
                hit.damage *= damageReductionMultiplier;
            })
            .DoOnCompleted(() =>
            {
                blockVfx.HideBlock();
                cooldown.ResetToZero();
                isHoldingBlock = false;
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

    public override void Tick()
    {
        base.Tick();

        if (isHoldingBlock)
            DrainEnergy(Time.deltaTime);
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
