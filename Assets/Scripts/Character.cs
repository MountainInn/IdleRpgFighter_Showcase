using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UniRx;
using UniRx.Triggers;

public class Character : Combatant
{
    public ObservableStateMachineTrigger ObserveStateMachine;

    void Start()
    {
        base.Construct(Stats);
    }

    public void EnterAttackState()
    {
        Debug.Log($"Enter Attack State");
        animator.SetTrigger(attackTriggerId);
    }
}
