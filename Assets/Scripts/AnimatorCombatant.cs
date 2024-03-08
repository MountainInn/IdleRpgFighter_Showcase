using UnityEngine;
using UnityEngine.Events;
using UniRx.Triggers;
using Zenject;

public abstract class AnimatorCombatant : Combatant
{
    [SerializeField] public Animator combatantAnimator;
    [SerializeField] protected string attackTriggerName = "attackTrigger";
    [SerializeField] public UnityEvent afterDeathAnimation;
    [SerializeField] public UnityEvent onAttackAnimEvent;

    protected int attackTriggerId;
    public ObservableStateMachineTrigger ObserveStateMachine;

    [Inject]
    public void Construct()
    {
        ObserveStateMachine = combatantAnimator.GetBehaviour<ObservableStateMachineTrigger>();
    }

    protected void Start()
    {
        attackTriggerId = Animator.StringToHash(attackTriggerName);

        onDie.AddListener(() => combatantAnimator.SetTrigger("death Trigger"));
    }

    public void InflictDamage_OnAnimEvent()
    {
        InflictDamage(target, stats.attackDamage);

        onAttackAnimEvent?.Invoke();
    }

    public void Die()
    {
        afterDeathAnimation?.Invoke();
    }
}
