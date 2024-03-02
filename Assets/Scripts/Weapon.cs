using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] public UnityEvent attackStart;
    [SerializeField] public UnityEvent attackEnd;
    [SerializeField] public UnityEvent preparationStart;
    [SerializeField] public UnityEvent preparationEnd;
    [SerializeField] protected HurtBox hurtbox;

    protected Combatant owner;
    protected Volume attackTimer;
    protected bool canTick;

    void Awake()
    {
        this.owner = GetComponent<Combatant>();

        // hurtbox.SetLayers(owner.gameObject.GetLayerMask(), owner.TargetLayers);

        Subscribe();
    }

    protected void Update()
    {
        if (canTick)
            AttackTimerTick(Time.deltaTime);
    }

    protected virtual void Subscribe()
    {
        ObserveCanAttack()
            .WhereEqual(true)
            .Subscribe(_ =>
            {
                attackTimer.ResetToZero();
            })
            .AddTo(this);

        SubscribeHurtbox();
    }

    protected void SubscribeHurtbox()
    {
        attackStart.AsObservable()
            .Subscribe(_ =>
            {
                // hurtbox.targets
                //     .ObserveAdd()
                //     .TakeUntil( attackEnd.AsObservable() )
                //     .Subscribe(ev =>
                //     {
                //         Attack(ev.Value);
                //     })
                //     .AddTo(this);
            })
            .AddTo(this);
    }

    protected IObservable<bool> ObserveCanAttack()
    {
        return
            Observable
            .CombineLatest( GetCanAttackObservables() )
            .Select(bools =>
                    bools.All(b => b == true));
    }

    protected virtual IEnumerable<IObservable<bool>> GetCanAttackObservables()
    {
        return new[]
        {
            owner.attackTimer.ObserveFull(),
            ObserveAttackNotOngoing(),
        };
    }

    protected virtual void Attack(Combatant target)
    {
        owner.InflictDamage(target, owner.Stats.attackDamage);
    }

    protected virtual Combatant SelectTarget(IEnumerable<Combatant> aliveTargets)
    {
        return
            aliveTargets
            .OrderBy(transform.DistanceSqrTo)
            .First();
    }

    public void AttackTimerTick(float deltaTime)
    {
        if (!attackTimer.IsFull)
        {
            attackTimer.Add(deltaTime);
        }
    }

    public IObservable<bool> ObservePreparationOngoing()
    {
        return
            Observable
            .Merge(preparationStart.AsObservable().Select(_ => true),
                   preparationEnd.AsObservable().Select(_ => false))
            .StartWith(false);
    }

    public IObservable<bool> ObservePreparationNotOngoing()
    {
        return ObservePreparationOngoing() .Select(b => !b);
    }

    public IObservable<bool> ObserveAttackOngoing()
    {
        return
            Observable
            .Merge(attackStart.AsObservable().Select(_ => true),
                   attackEnd.AsObservable().Select(_ => false))
            .StartWith(false);
    }

    public IObservable<bool> ObserveAttackNotOngoing()
    {
        return ObserveAttackOngoing() .Select(b => !b);
    }
}
