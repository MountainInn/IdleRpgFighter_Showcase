using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;

public class Rock : Combatant
{
    [SerializeField] MobStatsSO mobStatsSO;

    [Inject]
    public void Construct(FloatingTextSpawner floatingTextSpawner, MobView mobView)
    {
        Stats = mobStatsSO.ToStats();
        Stats.Apply(this);

        mobView.Subscribe(this);

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                floatingTextSpawner.FloatDamage(args);
            })
            .AddTo(this);
    }

    [Inject] void SubscribeToCharacter(Character character)
    {
        character.SetTarget(this);
    }

    [Inject] void SubscribeToDPSMeter(DPSMeter dpsMeter, DPSMeterView dpsView)
    {
        dpsMeter
            .ObserveDPS(this)
            .Subscribe(dpsView.SetText)
            .AddTo(this);
    }

    [Inject] void SubscribeToLootManager(LootManager lootManager)
    {
        lootManager.Subscribe(this);
    }

}
