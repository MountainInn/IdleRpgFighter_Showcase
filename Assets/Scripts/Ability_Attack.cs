using UniRx;

abstract public class Ability_Attack : Ability
{
    protected override void ConcreteSubscribe()
    {
        character
            .ObserveStateMachine
            .OnStateEnterAsObservable()
            .Subscribe(enter =>
            {
                if (enter.StateInfo.IsTag("attack"))
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
