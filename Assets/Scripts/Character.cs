using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UniRx;
using UniRx.Triggers;
using Zenject;

public class Character : Combatant
{
    [Inject] Mob mob;

    ObservableStateMachineTrigger ObserveStateMachine;

    int attackInQueue;
    bool isPlaying;

    void Start()
    {
        base.Construct(Stats);

        ObserveStateMachine = combatantAnimator.GetBehaviour<ObservableStateMachineTrigger>();

        ObserveStateMachine
            .OnStateExitAsObservable()
            .Subscribe(exit =>
            {
                bool isAttack = exit.StateInfo.IsName("standing melee attack downward");

                if (isAttack)
                {
                    InflictDamage(mob);

                    if (--attackInQueue > 0)
                    {
                        Debug.Log($"Sub {attackInQueue}");
                        combatantAnimator.SetTrigger(attackTriggerId);
                    }
                }
            })
            .AddTo(this);

        ObserveIsPlaying()
            .Subscribe(isPlaying => this.isPlaying = isPlaying)
            .AddTo(this);
    }

    private System.IObservable<bool> ObserveIsPlaying()
    {
        return
            Observable
            .CombineLatest(ObserveStateMachine.OnStateEnterAsObservable(),
                           ObserveStateMachine.OnStateExitAsObservable(),
                           (enter, exit) => enter.Equals(exit));
    }

    public void EnterAttackState()
    {
        attackInQueue++;

        Debug.Log($"Add {attackInQueue}");
        if (!isPlaying)
            combatantAnimator.SetTrigger(attackTriggerId);
    }
}
