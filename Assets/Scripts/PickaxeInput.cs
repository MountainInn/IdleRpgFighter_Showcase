using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Zenject;

public class PickaxeInput : MonoBehaviour, IDisposable
{
    [SerializeField] float baseDamage = 100;
    [SerializeField] float damageOnFullCharge = 100;
    [Space]
    [SerializeField] float maxCharge = 100;
    [SerializeField] float chargePerSecond = 50;
    [Space]
    [SerializeField] string pickaxeChargeFloatProperty;
    [SerializeField] string pickaxeHitTriggerProperty;

    [HideInInspector] public FloatReactiveProperty strikeDamage;

    Volume charge;
    ReadOnlyReactiveProperty<bool> isInIdleState;

    [Inject] Character character;
    [Inject]
    public void Construct(CharacterController charController,
                          Rock rock,
                          ProgressBar chargeProgressBar)
    {
        charge = new (0, maxCharge);
        strikeDamage = new();
        character.pickaxeInput = this;

        Button attackButton = charController.AttackButton;

        isInIdleState =
            character.ObserveStateMachine
            .OnStateEnterAsObservable()
            .Select(enter => enter.StateInfo.IsName("Idle"))
            .ToReadOnlyReactiveProperty(false);

        attackButton
            .OnPointerDownAsObservable()
            .Subscribe(pointerData =>
            {
                this.UpdateAsObservable()
                    .SkipUntil( isInIdleState.WhereEqual(true) )
                    .TakeUntil( attackButton.OnPointerUpAsObservable() )
                    .TakeUntil( charge.ObserveFull().WhereEqual(true) )
                    .DoOnCompleted(() =>
                    {
                        strikeDamage.Value =
                            baseDamage + damageOnFullCharge * charge.Ratio;

                        character
                            .combatantAnimator
                            .SetTrigger(pickaxeHitTriggerProperty);

                        charge.ResetToZero();
                    })
                    .Subscribe(_ =>
                    {
                        charge.Add(chargePerSecond * Time.deltaTime);
                    })
                    .AddTo(this);
            })
            .AddTo(this);

        charge
            .ObserveAll()
            .Subscribe(tuple =>
            {
                character
                    .combatantAnimator
                    .SetFloat(pickaxeChargeFloatProperty, tuple.ratio);
            })
            .AddTo(this);

        chargeProgressBar
            .Subscribe(gameObject, charge)
            .AddTo(this);
    }

    public void Dispose()
    {
        character.pickaxeInput = null;
    }
}
