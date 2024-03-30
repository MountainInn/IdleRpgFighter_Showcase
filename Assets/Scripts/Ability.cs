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

    protected IReadOnlyReactiveProperty<bool> isReadyToUse;

    public void SubscribeToCheats(Cheats cheats)
    {
        cheats.noCooldown
            .WhereEqual(true)
            .Subscribe(_ =>
            {
                cooldown
                    .ObserveFull()
                    .WhereEqual(false)
                    .TakeUntil(cheats.noCooldown.WhereEqual(false))
                    .Subscribe(_ => cooldown.Refill())
                    .AddTo(abilityButton);
            })
            .AddTo(abilityButton);
    }

    abstract protected void Use();

    virtual protected void ConcreteSubscribe()
    {
        abilityButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                Use();
                DrainEnergy();
                cooldown.ResetToZero();
            })
            .AddTo(abilityButton);
    }


    public void SubscribeButton(Character character,
                                AbilityButton abilityButton)
    {
        this.character = character;
        this.abilityButton = abilityButton;

        abilityButton.Connect(this);

        isReadyToUse =
            Observable
            .CombineLatest(cooldown.ObserveFull(),
                           ObserveHaveEnoughEnergy(),
                           (isReady, isEnough) => isReady && isEnough)
            .ToReactiveProperty();

        isReadyToUse
            .SubscribeToInteractable(abilityButton)
            .AddTo(abilityButton);

        ConcreteSubscribe();
    }

    protected override void Talent_SubInitialize()
    {
        cooldown = new(_cooldown);
    }

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
