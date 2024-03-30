using UniRx;
using UnityEngine;

abstract public class Ability_Attack : Ability
{
    [SerializeField] public string attackAnimationTrigger;
   
    public DamageArgs lastCreatedArgs;

    protected override void ConcreteSubscribe()
    {
        character
            .preAttack
            .AsObservable()
            .Subscribe(args =>
            {
                if (args == lastCreatedArgs)
                    DrainEnergy();
            })
            .AddTo(abilityButton);

        abilityButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                Use();
            })
            .AddTo(abilityButton);
    }
}
