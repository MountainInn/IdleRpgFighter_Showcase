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

    [HideInInspector] public ObservableStateMachineTrigger ObserveStateMachine;

    protected int attackTriggerId;

    [Inject]
    public void Construct()
    {
        InitObserveStateMachine();
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        combatantAnimator.runtimeAnimatorController = controller;
        InitObserveStateMachine();
    }
    public void InitObserveStateMachine()
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
        InflictDamage(target, Stats.attackDamage);

        onAttackAnimEvent?.Invoke();
    }

    public void Die()
    {
        afterDeathAnimation?.Invoke();
    }
}
