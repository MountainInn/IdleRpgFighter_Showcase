using UnityEngine;
using UnityEngine.Events;
using UniRx.Triggers;

public abstract class AnimatorCombatant : Combatant
{
    [SerializeField] public Animator combatantAnimator;
    [SerializeField] protected string attackTriggerName = "attackTrigger";
    [SerializeField] public UnityEvent afterDeathAnimation;

    protected int attackTriggerId;
    protected ObservableStateMachineTrigger ObserveStateMachine;

    protected void Start()
    {
        ObserveStateMachine = combatantAnimator.GetBehaviour<ObservableStateMachineTrigger>();

        attackTriggerId = Animator.StringToHash(attackTriggerName);

        onDie.AddListener(() => combatantAnimator.SetTrigger("death Trigger"));
    }

    public void InflictDamage_OnAnimEvent()
    {
        InflictDamage(target, stats.attackDamage);
    }

    public void Die()
    {
        afterDeathAnimation?.Invoke();
    }
}
