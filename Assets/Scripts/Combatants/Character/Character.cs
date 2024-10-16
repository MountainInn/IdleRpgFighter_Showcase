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
            .SubToggle(preAttack.AsObservable(),
                            args =>
                            args.damage = args.defender.health.maximum.Value)
            .AddTo(this);

        cheats.oneShotCharacter
            .SubToggle(preTakeDamage.AsObservable(),
                            args =>
                            args.damage = args.defender.health.maximum.Value)
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


    public void MaybeHitWithPickaxe_OnAnimEvent()
    {
        if (pickaxeInput == null)
            return;

        float damage = pickaxeInput.strikeDamage.Value;

        InflictDamage(target, damage);
    }
}
