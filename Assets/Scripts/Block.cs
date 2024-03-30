using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Block", menuName = "SO/Abilities/Block")]
public partial class Block : Ability
{
    [SerializeField] float parryTimeWindow;
    [Space]
    [SerializeField] bool canCounterAttack;
    [Space]
    [SerializeField] float counterAttackDamageBonus;
    [SerializeField] float bonusDuration;
    [SerializeField] List<Field> fields;

    [Serializable]
    struct Field
    {
        public float damageReductionFlat;
        public int price;
    }

    bool isHoldingBlock;

    BlockVfx blockVfx;
    AttackBonusVfx attackBonusVfx;
    AttackBuff attackBuff;

    float damageReductionFlat;
    float fortificationMult = 1;
    float energyDrainMult = 1;

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
                float blockValue = damageReductionFlat * fortificationMult;

                hit.damage = Mathf.Max(0, hit.damage - blockValue);
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
            DrainEnergy(energyCost * energyDrainMult,
                        Time.deltaTime);
    }

    public override IObservable<string> ObserveDescription()
    {
        return
            Observable.Return("*BLANK*");
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].price;

        damageReductionFlat = fields[level].damageReductionFlat;
    }
}
