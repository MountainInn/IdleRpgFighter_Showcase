using UnityEngine;
using Zenject;
using UniRx;
using System;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using UniRx.Diagnostics;

public class Mob : AnimatorCombatant
{
    [SerializeField] public MobStatsSO startingMobStats;
    [SerializeField] public float enrageChance;
    [SerializeField] public float relaxChance;
    [SerializeField] public float energyDrainPerAttack;
    [SerializeField] public float energyRegen;
   
    protected bool mobCanAttack;

    public BoolReactiveProperty isEnraged {get; protected set;} = new();

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;
    [Inject] GameSettings gameSettings;
    [Inject]
    public void SubscribeToCheats(Cheats cheats)
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

    IDisposable timerSubscription;

    [Inject]
    public void SubscribeEnrage()
    {
        onRespawn
            .AsObservable()
            .Subscribe(_ =>
            {
                gameSettings
                    .SubscribeToTimer(timerSubscription, this, onDie.AsObservable(),
                                      () =>
                                      {
                                          if (energy.current.Value >= energyDrainPerAttack &&
                                              !isEnraged.Value)
                                          {
                                              isEnraged.Value = (UnityEngine.Random.value <= enrageChance);
                                          }
                                      });
            })
            .AddTo(this);

        postAttack
            .AsObservable()
            .Subscribe(_ =>
            {
                if (isEnraged.Value)
                {
                    bool relaxRoll = (UnityEngine.Random.value <= relaxChance);
                    bool notEnoughtEnergy = (energy.maximum.Value < energyDrainPerAttack);

                    energy.Subtract(energyDrainPerAttack);

                    if (relaxRoll || notEnoughtEnergy)
                        isEnraged.Value = false;
                }

                if (isEnraged.Value)
                    attackTimer.Refill();
            })
            .AddTo(this);
    }

    protected new void Awake()
    {
        base.Awake();

        SubscribeToAttackTimerFull();

        postTakeDamage
            .AsObservable()
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
        if (mobCanAttack && CanContinueBattle() && isEnraged.Value)
            AttackTimerTick(Time.deltaTime);

        if (!energy.IsFull)
            energy.Add(Time.deltaTime * energyRegen);
    }
}
