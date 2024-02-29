using UniRx;
using System;
using UnityEngine;

[Serializable]
public class Price : IDropable
{
    public Currency currency;
    public IntReactiveProperty cost;

    public Sprite Sprite
    {
        get => currency.sprite;
        // To not allow to change sprite
        set => currency.sprite = Sprite;
    }

    public PickupImpl PickupImplementation
        => new PickupImpl_Coin();

    public Price(Currency currency, int cost)
    {
        this.currency = currency;
        this.cost = new IntReactiveProperty(cost);
    }

    public bool IsAffordable()
    {
        return cost.Value <= currency.amount.Value;
    }

    public IObservable<bool> IsAffordableObservable()
    {
        return
            Observable
            .CombineLatest(cost, currency.amount,
                           (cost, currency) => cost <= currency);
    }

    public IObservable<float> SavingProgressObservable()
    {
        return
            Observable
            .CombineLatest(cost, currency.amount,
                           (cost, currency) => (float)currency / cost);
    }


    public void Pay()
    {
        currency.amount.Value -= cost.Value;
    }

    public void GetPaid()
    {
        currency.amount.Value += cost.Value;
    }
}
