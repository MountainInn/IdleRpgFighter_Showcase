using UniRx;
using UnityEngine;
using Zenject;

public abstract class Ability : Talent, ITickable
{
    [SerializeField] protected Volume cooldown;

    protected AbilityButton abilityButton;

    [Inject]
    public void Construct(Character character, Battle battle,
                          AbilityButton abilityButton)
    {
        this.abilityButton = abilityButton;

        abilityButton.Connect(this);

        Observable
            .CombineLatest(cooldown.ObserveFull(),
                           battle.battleTarget,
                           (isFull, mob) => (isFull && mob != null))
            .SubscribeToInteractable(abilityButton)
            .AddTo(character);
    }

    public virtual void Tick()
    {
        if (!cooldown.IsFull)
            cooldown.Add(Time.deltaTime);
    }
}
