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
    int attackInQueue;
    bool isPlaying;

    void Start()
    {
        base.Construct(Stats);

        ObserveStateMachine
            .OnStateExitAsObservable()
            .Subscribe(exit =>
            {
                bool isAttack = exit.StateInfo.IsName("standing melee attack downward");

                if (isAttack)
                {
                    InflictDamage(target);

                    if (--attackInQueue > 0)
                    {
                        combatantAnimator.SetTrigger(attackTriggerId);
                    }
                }
            })
            .AddTo(this);

        ObserveIsPlaying()
            .Subscribe(isPlaying => this.isPlaying = isPlaying)
            .AddTo(this);

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                Debug.Log($"You Ded");
            })
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

        if (!isPlaying)
            combatantAnimator.SetTrigger(attackTriggerId);
    }
}
