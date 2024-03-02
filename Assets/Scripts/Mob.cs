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

        weapon.SetAttackTimer(attackTimer);

        healthBar.Subscribe(gameObject, health);
        attackTimerBar.Subscribe(gameObject, attackTimer);

        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(attackTriggerId))
            .AddTo(this);
    }



    public void ReturnToPool()
    {
        onAskedToReturnToPool.Invoke();
    }
}
