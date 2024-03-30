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

    bool isHoldingBlock;

    BlockVfx blockVfx;
    AttackBonusVfx attackBonusVfx;
    AttackBuff attackBuff;

    public void Subscribe(BlockVfx blockVfx, AttackBonusVfx attackBonusVfx)
    {
        this.blockVfx = blockVfx;
        this.attackBonusVfx = attackBonusVfx;
    }

    protected override void ConcreteSubscribe()
    {
        attackBuff = new(){ duration = bonusDuration,
                            multiplier = counterAttackDamageBonus };

        attackBuff.Subscribe(character);

        abilityButton
            .OnPointerDownAsObservable()
            .Subscribe(_ =>
            {
                if (isReadyToUse.Value)
                {
                    Use();
                }
            })
            .AddTo(abilityButton);
    }

    protected override void Use()
    {
        SubscribeBlock(character);
        SubscribeParry(character);
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
                isHoldingBlock = false;
            })
            .Subscribe()
            .AddTo(abilityButton);
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
                    attackBuff.StartBuff(character.gameObject);
                }
            })
            .DoOnCompleted(() =>
            {
                blockVfx.HideParry();
            })
            .Subscribe()
            .AddTo(abilityButton);
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
