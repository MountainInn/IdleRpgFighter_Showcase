using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

abstract public class Combatant : MonoBehaviour
{
    [SerializeField] public Volume health;
    [SerializeField] public Volume attackTimer;
    [Space]
    [SerializeField] public UnityEvent onDie;
    [SerializeField] public UnityEvent onRespawn;
    [SerializeField] public UnityEvent<Combatant> onKill;
    [Space]
    [HideInInspector] public DropList dropList;

    [HideInInspector] [SerializeField] public StatsSO Stats;

    [InjectOptional] protected Combatant target;

    [HideInInspector]
    public UnityEvent<DamageArgs>
        preAttack,
        preTakeDamage,
        postTakeDamage,
        postAttack;

    protected void OnEnable()
    {
        health.Refill();
    }

    public bool AttackTimerTick(float delta)
    {
        attackTimer.Add(delta);

        bool isFull = attackTimer.IsFull;

        if (isFull)
            attackTimer.ResetToZero();

        return isFull;
    }

    public void InflictDamage(Combatant defender)
    {
        InflictDamage(defender, Stats.attackDamage);
    }

    public void InflictDamage(Combatant defender, float damage)
    {
        if (!defender.IsAlive)
            return;
       
        DamageArgs args = new DamageArgs()
        {
            attacker = this,
            defender = defender,
            damage = damage
        };

        preAttack?.Invoke(args);

        defender.TakeDamage(args);

        postAttack?.Invoke(args);

        if (!defender.IsAlive)
        {
            onKill?.Invoke(defender);

            defender.onDie?.Invoke();
        }
    }

    public void TakeDamage(DamageArgs args)
    {       
        preTakeDamage?.Invoke(args);

        health.Subtract(args.damage);

        postTakeDamage?.Invoke(args);
    }

    public void Respawn()
    {
        health.Refill();
        attackTimer.ResetToZero();

        onRespawn?.Invoke();
    }

    protected bool CanContinueBattle()
    {
        return IsAlive && target.IsAlive;
    }

    public bool IsAlive => health.current.Value > 0;
}
