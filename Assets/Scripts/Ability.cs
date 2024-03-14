using UniRx;
using UnityEngine;
using Zenject;

public abstract class Ability : Talent, ITickable
{
    [SerializeField] protected Volume cooldown;

    protected AbilityButton abilityButton;

    [Inject]
    public void Construct(Character character, AbilityButton abilityButton)
    {
        this.abilityButton = abilityButton;

        abilityButton.Connect(this);

        cooldown.ObserveFull()
            .SubscribeToInteractable(abilityButton)
            .AddTo(character);
    }

    public virtual void Tick()
    {
        if (!cooldown.IsFull)
            cooldown.Add(Time.deltaTime);
    }
}
