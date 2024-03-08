using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;
using DG.Tweening;

public partial class Mob : AnimatorCombatant
{
    [SerializeField] public UnityEvent onAskedToReturnToPool;

    MobStatsSO mobStats;
    public MobStatsSO MobStats => mobStats;

    bool mobCanAttack;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;

    void Awake()
    {
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

    [Inject] void SubToView(MobView mobView)
    {
        mobView.Subscribe(this);

        var fade = mobView.GetComponent<Fade>();

        afterDeathAnimation.AddListener(fade.FadeOut);
        onRespawn.AddListener(fade.FadeIn);
    }

    public void SetStats(MobStatsSO mobStats)
    {
        base.SetStats(mobStats);

        this.mobStats = mobStats;
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
