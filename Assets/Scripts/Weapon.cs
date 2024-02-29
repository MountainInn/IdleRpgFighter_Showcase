using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponStatSO stats;
    [SerializeField] protected Sights_Combatant sights;
    [SerializeField] public UnityEvent attackStart;
    [SerializeField] public UnityEvent attackEnd;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected HurtBox hurtbox;

    public Sights_Combatant Sights => sights;

    protected Combatant owner;
    protected Volume attackTimer;
    protected bool canTick;

    protected void OnValidate()
    {
        sights ??= GetComponentInChildren<Sights_Combatant>();
        if (sights == null)
        {
            sights = new GameObject("Sights").AddComponent<Sights_Combatant>();

            sights.transform.SetParent(transform);
            sights.transform.localPosition = Vector3.zero;
            sights.transform.localScale = Vector3.one;
        }
    }

    [Inject]
    public void Construct(Combatant owner)
    {
        this.owner = owner;

        stats = Instantiate(stats);
        stats.Multiply(owner.Stats);

        Sights.SetLayers(owner.gameObject.GetLayerMask(), owner.TargetLayers);
        Sights.Collider.radius = stats.attackRange;
       
        attackTimer = new Volume(stats.attackSpeed);

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
                var aliveTargets = sights.targets.Where(m => m.IsAlive);

                if (aliveTargets.Any())
                {
                    Combatant closestTarget = SelectTarget(aliveTargets);

                    Attack(closestTarget);

                    attackTimer.ResetToZero();
                }
            })
            .AddTo(this);

        SubscribeHurtbox();
    }

    protected void SubscribeHurtbox()
    {
        attackStart.AsObservable()
            .Subscribe(_ =>
            {
                hurtbox.targets
                    .ObserveAdd()
                    .TakeUntil( attackEnd.AsObservable() )
                    .Subscribe(ev =>
                    {
                        owner.InflictDamage(ev.Value, stats.attackDamage);
                    })
                    .AddTo(this);
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
            attackTimer.ObserveFull(),
            sights.ObserveHasTargets(),
            ObserveAttackNotOngoing(),
        };
    }

    protected virtual void Attack(Combatant target)
    {
        owner.InflictDamage(target, stats.attackDamage);
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
