using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Zenject;
using System.Collections.Generic;
using System;

public class Character : AnimatorCombatant
{
    [SerializeField] CharacterStatsSO characterStatsSO;
    [SerializeField] public Volume energy;

    [HideInInspector] public PickaxeInput pickaxeInput;

    List<ITickable> tickables = new();

    [Inject] DiContainer Container;
    [Inject]
    public void Construct(LevelSwitcher levelSwitcher)
    {
        energy = new();

        levelSwitcher.AddSceneLoadedCallback(() => Respawn());

        characterStatsSO . ToStats() . Apply(this);
    }

    [Inject]
    public void SubscribeToCheats(Cheats cheats)
    {
        cheats.oneShotMob
            .WhereEqual(true)
            .Subscribe(_ =>
            {
                preAttack
                    .AsObservable()
                    .TakeUntil(cheats.oneShotMob.WhereEqual(false))
                    .Subscribe(args =>
                               args.damage = args.defender.health.maximum.Value)
                    .AddTo(this);
            })
            .AddTo(this);

        cheats.oneShotCharacter
            .WhereEqual(true)
            .Subscribe(toggle =>
            {
                preTakeDamage
                    .AsObservable()
                    .TakeUntil(cheats.oneShotCharacter.WhereEqual(false))
                    .Subscribe(args =>
                               args.damage = args.defender.health.maximum.Value)
                    .AddTo(this);
            })
            .AddTo(this);

        cheats.godMode
            .Subscribe(toggle =>
            {
                if (toggle)
                    health.ResizeAndRefill(int.MaxValue);
                else
                    health.ResizeAndRefill(characterStatsSO.health);
            })
            .AddTo(this);
    }

    public void AddTickable(ITickable tickable)
    {
        tickables.Add(tickable);
    }

    bool isPlaying;
    new void Start()
    {
        base.Start();

        InitObserveStateMachine();

        ObserveIsPlaying()
            .Subscribe(isPlaying => this.isPlaying = isPlaying)
            .AddTo(this);
    }

    void Update()
    {
        foreach (var item in tickables)
        {
            item.Tick();
        }
    }

    public void SetTarget(Combatant target)
    {
        this.target = target;
    }

    public System.IObservable<bool> ObserveIsPlaying()
    {
        return
            Observable
            .CombineLatest(ObserveStateMachine.OnStateEnterAsObservable(),
                           ObserveStateMachine.OnStateExitAsObservable(),
                           (enter, exit) => enter.Equals(exit));
    }

    public void Attack()
    {
        if (!CanContinueBattle() || isPlaying)
            return;

        combatantAnimator.SetTrigger(attackTriggerId);
    }

    public void MaybeHitWithPickaxe_OnAnimEvent()
    {
        if (pickaxeInput == null)
            return;

        float damage = pickaxeInput.strikeDamage.Value;

        InflictDamage(target, damage);
    }
}
