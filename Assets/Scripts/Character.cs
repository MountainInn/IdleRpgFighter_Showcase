using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Zenject;

public class Character : AnimatorCombatant
{
    [SerializeField] CharacterStatsSO characterStatsSO;

    [HideInInspector] public PickaxeInput pickaxeInput;

    [Inject] DiContainer Container;
    [Inject]
    public void Construct(LevelSwitcher levelSwitcher)
    {
        levelSwitcher.AddSceneLoadedCallback(() => Respawn());

        characterStatsSO . ToStats() . Apply(this);
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
