using UnityEngine;
using System;
using Zenject;
using System.Collections.Generic;

public abstract class Talent : ScriptableObject
{
    [SerializeField] public Sprite sprite;
    [SerializeField] [HideInInspector] protected int maxLevel;
    [SerializeField] [HideInInspector] protected List<Field> cost = new();

    [HideInInspector] public Buyable<Level> buyableLevel;

    public int Level => buyableLevel.ware.level.Value;

    protected abstract void OnLevelUp(int level, Price price);

    protected void CostUp(int level, Price price)
    {
        price.cost.Value = cost[level];
    }

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

    [Inject]
    protected void InitializeBuyableLevel(Vault vault)
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

    [Serializable]
    protected struct Field
    {
        public int intValue;

        static public implicit operator int(Field field)
        {
            return field.intValue;
        }
    }
}
