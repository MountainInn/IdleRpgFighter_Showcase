using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using DG.Tweening;

public partial class Mob : Combatant
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] public UnityEvent onAskedToReturnToPool;
    [SerializeField] public UnityEvent onEnterPreparationState;
    [SerializeField] public UnityEvent onExitPreparationState;
    [Space]
    [Header("Mob")]
    [SerializeField] StatsSO mobStats;

    public StatsSO MobStats => mobStats;

    Color baseColor;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;

    [Inject] public void Construct(Character character, MobView mobView)
    {
        base.Construct(mobStats);

        this.mobStats = (StatsSO)this.stats;

        mobView.Subscribe(this);

        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(attackTriggerId))
            .AddTo(this);


        ObserveStateMachine
            .OnStateExitAsObservable()
            .Subscribe(exit =>
            {
                bool isAttack = exit.StateInfo.IsName("standing melee attack downward");

                if (isAttack)
                {
                    Debug.Log($"{target}");
                    InflictDamage(target);
                }
            })
            .AddTo(this);

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);
    }

    void Update()
    {
        AttackTimerTick(Time.deltaTime);
    }

    public void ReturnToPool()
    {
        onAskedToReturnToPool.Invoke();
    }
}
