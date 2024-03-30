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

    [HideInInspector] public ObservableStateMachineTrigger ObserveStateMachine
    {
        get {
            if (_ObserveStateMachine == null)
                InitObserveStateMachine();

            return _ObserveStateMachine;
        }

        protected set => _ObserveStateMachine = value;
    }
    ObservableStateMachineTrigger _ObserveStateMachine;

    protected int basicAttackTriggerId;

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
        basicAttackTriggerId = Animator.StringToHash(attackTriggerName);

        onDie.AddListener(() => combatantAnimator.SetBool("death", true));
        onRespawn.AddListener(() => combatantAnimator.SetBool("death", false));
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
