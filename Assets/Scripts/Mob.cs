using UnityEngine;
using Zenject;
using UniRx;
using System;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class Mob : AnimatorCombatant
{
    [SerializeField] public MobStatsSO startingMobStats;

    protected bool mobCanAttack;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;
    [Inject]
    public void Construct(Cheats cheats)
    {
        cheats.mobOneSecondAttackTimer
            .SubToggle(onStatsApplied.AsObservable(),
                       _ => attackTimer.Resize(1))
            .AddTo(this);

        cheats.trainingDummy
            .SubToggle(onStatsApplied.AsObservable(),
                       _ => health.ResizeAndRefill(int.MaxValue))
            .AddTo(this);

        onStatsApplied
            .AsObservable()
            .Take(1)
            .Subscribe(_ =>
            {
                cheats.mobOneSecondAttackTimer
                    .Subscribe(toggle => attackTimer.Resize(toggle ? 1 : Stats.attackTimer))
                    .AddTo(this);

                cheats.trainingDummy
                    .Subscribe(toggle => health.ResizeAndRefill(toggle ? int.MaxValue : Stats.health))
                    .AddTo(this);
            })
            .AddTo(this);
    }

    protected new void Awake()
    {
        base.Awake();

        SubscribeToAttackTimerFull();

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);

        SubscribeCanAttack();

        Respawn();
    }

    protected void SubscribeToAttackTimerFull()
    {
        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(basicAttackTriggerId))
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

    [Inject] void SubscribeToLootManager(LootManager lootManager, NominalParticles nominalParticles)
    {
        lootManager.Subscribe(this, nominalParticles);
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
