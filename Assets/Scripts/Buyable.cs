using System.Linq;
using UniRx;
using System.Collections.Generic;
using System;

public class Buyable<T>
{
    public List<Price> prices;
    public Action onBuy;
    public T obj;

    public Buyable(Action onBuy, Price[] prices, T obj)
    {
        this.prices = prices.ToList();
        this.obj = obj;
        this.onBuy = onBuy;
    }

    public bool IsAffordable()
    {
        return prices.All(price => price.IsAffordable());
    }

    public IObservable<bool> IsAffordableObservable()
    {
        return
            Observable
            .CombineLatest(
                prices
                .Select(price => price.IsAffordableObservable())
            )
            .Select(affordables => affordables.All(a => a == true));
    }

    public IObservable<float> SavingProgressObservable()
    {
        return
            Observable
            .CombineLatest(
                prices
                .Select(price => price.SavingProgressObservable())
            )
            .Select(savings =>
                    savings.Sum() / prices.Count);
    }

    public void Buy()
    {
        prices.Map(price => price.Pay());
        onBuy?.Invoke();
    }
}
