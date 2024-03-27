using System;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class Ability : Talent, ITickable
{
    [SerializeField] protected float _cooldown;
    [SerializeField] protected float energyCost;

    [HideInInspector] protected Volume cooldown = new();

    protected AbilityButton abilityButton;

    protected Character character;

    public void SubscribeButton(Character character,
                                AbilityButton abilityButton)
    {
        this.character = character;
        this.abilityButton = abilityButton;

        abilityButton.Connect(this);

        Observable
            .CombineLatest(cooldown.ObserveFull(),
                           ObserveHaveEnoughEnergy(),
                           (isReady, isEnough) => isReady && isEnough)
            .SubscribeToInteractable(abilityButton)
            .AddTo(character);

        ConcreteSubscribe();
    }

    protected override void Talent_SubInitialize()
    {
        cooldown = new(_cooldown);
    }

    abstract protected void ConcreteSubscribe();

    public IObservable<bool> ObserveHaveEnoughEnergy(float timeDelta = 1)
    {
        return
            character.energy
            .ObserveAll()
            .Select(tuple => (tuple.current >= energyCost * timeDelta));
    }

    protected void DrainEnergy(float deltaTime = 1)
    {
        character.energy.Subtract(energyCost * deltaTime);
    }

    public virtual void Tick()
    {
        if (!cooldown.IsFull)
            cooldown.Add(Time.deltaTime);
    }
}
