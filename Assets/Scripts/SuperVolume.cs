using System.Linq;
using UniRx;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.SerializableAttribute]
public class SuperVolume : Volume
{
    public List<Volume> subvolumes { get; protected set; }

    public IObservable<(Volume volume, bool isFull, int index)> ObserveSubvolumeFull() =>
        Observable
        .Merge(subvolumes.Select((v, i) =>
                                 v.ObserveFull()
                                 .Select(isFull => (v, isFull, i))));

    public SuperVolume(IEnumerable<int> sublengths)
    {
        subvolumes = sublengths.Select(l => new Volume(0, l)).ToList();
        _current = new FloatReactiveProperty(0);
        _maximum = new FloatReactiveProperty(sublengths.Sum());
    }

    public new void Add(float amount)
    {
        while (amount > 0 &&
               TryGetFirstNonFullVolume(out Volume v))
        {
            v.Add(amount, out amount);
        }

        current.Value = subvolumes.Sum(v => v.current.Value);
    }

    bool TryGetFirstNonFullVolume(out Volume volume)
    {
        volume = subvolumes.FirstOrDefault(v => !v.IsFull);

        return volume != null;
    }
}
