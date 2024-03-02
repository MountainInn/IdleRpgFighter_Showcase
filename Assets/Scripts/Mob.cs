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
    [SerializeField] public UnityEvent onDied;
    [SerializeField] public UnityEvent onAskedToReturnToPool;
    [SerializeField] public UnityEvent onEnterPreparationState;
    [SerializeField] public UnityEvent onExitPreparationState;
    [Space]
    [Header("Mob")]
    [SerializeField] StatsSO mobStats;
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar attackTimerBar;
    [SerializeField] FloatingTextSpawner floatingTextSpawner;
    [Header("Weapon")]
    [SerializeField] ChargeWeapon weapon;

    public StatsSO MobStats => mobStats;

    Color baseColor;

    [Inject]
    public void Construct(Character character)
    {
        base.Construct(mobStats);

        this.mobStats = (StatsSO)this.stats;

        healthBar.Subscribe(gameObject, health);
        attackTimerBar.Subscribe(gameObject, attackTimer);

        health.ObserveChange()
            .Subscribe(change =>
            {
                floatingTextSpawner.Float(change.ToString("F1"));
            })
            .AddTo(this);

        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => animator.SetTrigger(attackTriggerId))
            .AddTo(this);
    }

    public void EnterPreparationState()
    {
        weapon.preparationStart.Invoke();
    }

    public void EnterAttackState()
    {
        weapon.preparationEnd.Invoke();
    }

    public void EnterDamagedState()
    {
        weapon.preparationEnd.Invoke();

    }


    public void ReturnToPool()
    {
        onAskedToReturnToPool.Invoke();
    }
}
