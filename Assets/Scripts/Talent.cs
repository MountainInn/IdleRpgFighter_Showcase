using UnityEngine;
using System;
using Zenject;

public abstract class Talent : ScriptableObject
{
    [SerializeField] public Sprite sprite;

    [HideInInspector] public Buyable<Level> buyableLevel;


    protected abstract void OnLevelUp(int level, Price price);

    public abstract IObservable<string> ObserveDescription();


    [Inject] protected void InitializeBuyableLevel(Vault vault)
    {
        Price price = new Price(vault.gold);
        Level level = new Level(l => OnLevelUp(l, price));

        buyableLevel = new Buyable<Level>(level,
                                          level => level.Up(),
                                          price);
    }

    [Inject] protected void ConnectToView(TalentView talentView)
    {
        talentView.ConnectBase(this);
    }
}
