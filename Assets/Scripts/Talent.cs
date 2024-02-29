using UnityEngine;
using System;
using Zenject;

public abstract class Talent : ScriptableObject
{
    [SerializeField] public Sprite sprite;

    [HideInInspector] public Buyable<Level> buyableLevel;

    [Inject] public void Construct(Vault vault, TalentView talentView)
    {
        InitializeBuyableLevel(vault);

        ConnectToView(talentView);
    }

    protected abstract void OnLevelUp(int level, Price price);
    public abstract IObservable<string> ObserveDescription();

    protected void ConnectToView(TalentView talentView)
    {
        talentView.ConnectBase(this);
    }

    protected void InitializeBuyableLevel(Vault vault)
    {
        Price price = new Price(vault.souls);
        Level level = new Level(l => OnLevelUp(l, price));

        buyableLevel = new Buyable<Level>(level,
                                          level => level.Up(),
                                          price);
    }
}
