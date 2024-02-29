using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;

abstract public class Combatant : MonoBehaviour
{
    [SerializeField] public Volume health;
    [SerializeField] public Volume attackTimer;
    [Space]
    [SerializeField] protected StatsSO stats;
    [Space]
    [SerializeField] public UnityEvent onDie;
    [SerializeField] public UnityEvent<Combatant> onKill;
    [SerializeField] public FloatingTextSpawner floatingTextSpawner;

    [HideInInspector] public int defense;

    public StatsSO Stats => stats;


    public UnityEvent<DamageArgs>
        preAttack,
        preTakeDamage,
        postTakeDamage,
        postAttack;

    public void Construct(StatsSO stats)
    {
        this.stats = Instantiate(stats);

        health.ResizeAndRefill(stats.health);
        attackTimer.Resize(stats.attackSpeed);

        // preAttack += (args) =>
        // {
        // stats.attackEffectSystem.Play();
        // };

        health.ObserveChange()
            .Subscribe(change =>
            {
                floatingTextSpawner.Float(change.ToString("F1"));
            })
            .AddTo(this);
    }

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
        InflictDamage(defender, stats.attackDamage);
    }

    public void InflictDamage(Combatant defender, float damage)
    {
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

    public bool IsAlive => health.current.Value > 0;
}
