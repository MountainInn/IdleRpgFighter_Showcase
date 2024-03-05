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


    Color baseColor;
    bool mobCanAttack;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;

    [Inject] public void Construct(Character character, MobView mobView)
    {
        base.Construct();
       
        mobView.Subscribe(this);

        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(attackTriggerId))
            .AddTo(this);

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);


        mobCanAttack = true;
        onDie.AddListener(() => mobCanAttack = false);
        onRespawn.AddListener(() => mobCanAttack = true);
    }

    public void SetStats(MobStatsSO mobStats)
    {
        base.SetStats(mobStats);

        mobStats.template.ApplyTemplate(gameObject);
    }

    void Update()
    {
        if (mobCanAttack && CanContinueBattle())
            AttackTimerTick(Time.deltaTime);
    }

    public void ReturnToPool()
    {
        onAskedToReturnToPool.Invoke();
    }
}
