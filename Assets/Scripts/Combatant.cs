using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx.Triggers;
using Zenject;

abstract public class Combatant : MonoBehaviour
{
    [SerializeField] public Volume health;
    [SerializeField] public Volume attackTimer;
    [SerializeField] protected LayerMask targetLayers;
    [Space]
    [SerializeField] protected StatsSO stats;
    [Space]
    [SerializeField] public UnityEvent onDie;
    [SerializeField] public UnityEvent afterDeathAnimation;
    [SerializeField] public UnityEvent onRespawn;
    [SerializeField] public UnityEvent<Combatant> onKill;
    [HeaderAttribute("Animations")]
    [SerializeField] public Animator combatantAnimator;
    [SerializeField] protected string attackTriggerName;

    [HideInInspector] public int defense;

    [Inject] protected Combatant target;

    public StatsSO Stats => stats;
    public LayerMask TargetLayers => targetLayers;


    public UnityEvent<DamageArgs>
        preAttack,
        preTakeDamage,
        postTakeDamage,
        postAttack;

    protected int attackTriggerId;

    protected ObservableStateMachineTrigger ObserveStateMachine;

    public void Construct()
    {
        ObserveStateMachine = combatantAnimator.GetBehaviour<ObservableStateMachineTrigger>();

        attackTriggerId = Animator.StringToHash(attackTriggerName);

        onDie.AddListener(() => combatantAnimator.SetTrigger("death Trigger"));
    }

    public void SetStats(StatsSO stats)
    {
        this.stats = Instantiate(stats);

        health.ResizeAndRefill(stats.health);
        attackTimer.ResetToZero();
        attackTimer.Resize(stats.attackSpeed);
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

    public void InflictDamage_OnAnimEvent()
    {
        InflictDamage(target, stats.attackDamage);
    }

    public void InflictDamage(Combatant defender)
    {
        InflictDamage(defender, stats.attackDamage);
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

    public void Die()
    {
        afterDeathAnimation?.Invoke();
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
