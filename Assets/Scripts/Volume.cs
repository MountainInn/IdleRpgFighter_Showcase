using UnityEngine;
using System.Linq;
using UniRx;
using System;

[System.SerializableAttribute]
public class Volume
{
    public FloatReactiveProperty
        maximum,
        current;


    public float Unfilled => maximum.Value - current.Value;
    public bool IsFullZeroes =>
        current.Value == 0 && maximum.Value == 0;


    public Volume(float current)
        :this(current, current)
    {

    }

    public Volume(float current, float maximum)
    {
        this.current = new FloatReactiveProperty(current);
        this.maximum = new FloatReactiveProperty(maximum);
    }

    public IObservable<(float current, float maximum, float ratio)> ObserveAll() =>
        Observable.CombineLatest(
            current, maximum,
            (cur , max) => (cur, max, Ratio));

    public IObservable<bool> ObserveFull() =>
        Observable.CombineLatest(
            current, maximum,
            (cur , max) => !IsFullZeroes && IsFull);

    public IObservable<bool> ObserveEmpty() =>
        current
        .Select(v => !IsFullZeroes && IsEmpty);

    public IObservable<bool> ObserveRefill() =>
        ObserveFull()
        .Pairwise()
        .Select(fulls =>
                !IsFullZeroes &&
                fulls.Previous == false && fulls.Current == true);

    public IObservable<float> ObserveChange() =>
        current
        .Pairwise((a, b) => b - a);

    public bool IsFull => (current.Value == maximum.Value);
    public bool IsEmpty => (current.Value == 0);

    public void ResetToZero()
    {
        current.Value = 0;
    }

    public float Ratio => (current.Value / maximum.Value);

    public void ResetTo(float newCurrentAmount)
    {
        current.Value = newCurrentAmount;
    }

    public void Add(float amount, out float overflow)
    {
        overflow = 0;

        if (amount > Unfilled)
            overflow = amount - Unfilled;

        Add(amount);
    }
    public void Add(float amount)
    {
        amount = Mathf.Min(amount, Unfilled);
        current.Value = Mathf.Clamp(current.Value + amount,
                                    0, maximum.Value);
    }

    public void Subtract(float amount, out float overflow)
    {
        overflow = 0;

        if (amount > current.Value)
            overflow = amount - current.Value;

        Subtract(amount);
    }

    public void Subtract(float amount)
    {
        current.Value -= Mathf.Min(amount, current.Value);
    }

    public void Reinitialize(float current, float maximum)
    {
        this.current.Value = current;
        this.maximum.Value = maximum;
    }

    public void ResizeAndRefill(float newMaximum)
    {
        maximum.Value = newMaximum;
        current.Value = newMaximum;
    }

    public void Resize(float newMaximum)
    {
        maximum.Value = newMaximum;
        current.Value = Clamp(current.Value);
    }

    public void Refill()
    {
        current.Value = maximum.Value;
    }

    float Clamp(float amount)
    {
        return Mathf.Clamp(amount, 0, maximum.Value);
    }

    public override string ToString()
    {
        return $"{current.Value}/{maximum.Value}";
    }
}
