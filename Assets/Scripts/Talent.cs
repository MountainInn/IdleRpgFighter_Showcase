using UnityEngine;
using System;
using Zenject;

public abstract class Talent : ScriptableObject
{
    [SerializeField] public Sprite sprite;

    [HideInInspector] public Buyable<Level> buyableLevel;


    protected abstract void OnLevelUp(int level, Price price);

    public abstract IObservable<string> ObserveDescription();

    [Inject]
    public void RegisterWithSaveSystem(SaveSystem saveSystem)
    {
        saveSystem
            .MaybeRegister<int>(this,
                                $"{name}:level",
                                () => buyableLevel.ware.level.Value,
                                (val) => buyableLevel.ware.SetLevel(val));
    }

    [Inject] protected void InitializeBuyableLevel(Vault vault)
    {
        Talent_SubInitialize();

        Price price = new Price(vault.gold);
        Level level = new Level(l => OnLevelUp(l, price));

        buyableLevel = new Buyable<Level>(level,
                                          level => level.Up(),
                                          price);
    }

    protected virtual void Talent_SubInitialize()
    {

    }

    [Inject] protected void ConnectToView(TalentView talentView)
    {
        talentView.ConnectBase(this);
    }
}
