using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Zenject;

public class PickaxeInput : MonoBehaviour
{
    [SerializeField] float damageOnFullCharge = 100;
    [SerializeField] float chargePerSecond = 25;
    [SerializeField] float maxCharge = 100;
    [Space]
    [SerializeField] string pickaxeChargeFloatProperty;
    [SerializeField] string pickaxeHitTriggerProperty;

    [HideInInspector] public FloatReactiveProperty strikeDamage;

    Volume charge;
    ReadOnlyReactiveProperty<bool> isInIdleState;

    [Inject]
    public void Construct(Character character,
                          CharacterController charController,
                          Rock rock,
                          MobView rockView,
                          ProgressBar chargeProgressBar,
                          FloatingTextSpawner floatingTextSpawner)
    {
        charge = new (0, maxCharge);
        strikeDamage = new();

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
                        strikeDamage.Value = charge.Ratio * damageOnFullCharge;

                        character
                            .combatantAnimator
                            .SetTrigger(pickaxeHitTriggerProperty);

                        character
                            .combatantAnimator
                            .SetFloat(pickaxeChargeFloatProperty, 0);

                        charge.ResetToZero();
                    })
                    .Subscribe(_ =>
                    {
                        charge.Add(chargePerSecond * Time.deltaTime);

                        character
                            .combatantAnimator
                            .SetFloat(pickaxeChargeFloatProperty, charge.Ratio);
                    })
                    .AddTo(this);
            })
            .AddTo(this);

        chargeProgressBar
            .Subscribe(gameObject, charge)
            .AddTo(this);

        rock
            .postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                floatingTextSpawner.FloatDamage(args);
            })
            .AddTo(this);

        rockView.Subscribe(rock);
    }
}
