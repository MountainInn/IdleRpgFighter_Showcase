using UnityEngine;
using Zenject;
using UniRx;
using System;
using UnityEngine.Events;

public class Mob : AnimatorCombatant
{
    [SerializeField] public MobStatsSO startingMobStats;

    protected bool mobCanAttack;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;

    protected void Awake()
    {
        SubscribeToAttackTimerFull();

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);

        SubscribeCanAttack();
    }

    protected void SubscribeToAttackTimerFull()
    {
        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(attackTriggerId))
            .AddTo(this);
    }

    protected void SubscribeCanAttack()
    {
        mobCanAttack = true;
        onDie.AddListener(() => mobCanAttack = false);
        onRespawn.AddListener(() => mobCanAttack = true);
    }

    [Inject] void SubToView(MobView mobView)
    {
        mobView.Subscribe(this);

        var fade = mobView.GetComponent<Fade>();

        afterDeathAnimation.AddListener(() => fade.FadeOut());
        onRespawn.AddListener(() => fade.FadeIn());
    }

    [Inject] void SubscribeToCharacter(Character character)
    {
        character.SetTarget(this);
    }

    [Inject] void SubscribeToDPSMeter(DPSMeter dpsMeter, DPSMeterView dpsView)
    {
        dpsMeter
            .ObserveDPS(this)
            .Subscribe(dpsView.SetText)
            .AddTo(this);
    }

    [Inject] void SubscribeToLootManager(LootManager lootManager)
    {
        lootManager.Subscribe(this);
    }

    [Inject] void SubscribeToGang(Gang gang)
    {
        gang.Initialize(this, (Character)target);
    }

    public void Update()
    {
        if (mobCanAttack && CanContinueBattle())
            AttackTimerTick(Time.deltaTime);
    }
}
