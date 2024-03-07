using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Zenject;

public class Character : Combatant
{
    [Header("Speedup")]
    [SerializeField] int clicksToSpeedupLevel = 2;
    [SerializeField] float speedupPerLevel = 0.2f;
    [Space]
    [SerializeField] string attackAnimationTag = "attack";
    [SerializeField] string attackSpeedParameter = "speed";

    [Inject] List<Talent> talents;

    int attackInQueue;
    bool isPlaying;
    int attackSpeedParameterId;
    int attackAnimationTagId;

    void Awake()
    {
        base.Construct();

        SetStats(Stats);

        attackSpeedParameterId = Animator.StringToHash(attackSpeedParameter);
        attackAnimationTagId = Animator.StringToHash(attackAnimationTag);

        ObserveIsPlaying()
            .Subscribe(isPlaying => this.isPlaying = isPlaying)
            .AddTo(this);

        onDie.AsObservable()
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
        if (!CanContinueBattle())
            return;

        if (!isPlaying)
            combatantAnimator.SetTrigger(attackTriggerId);
    }

}
