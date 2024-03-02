using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using System.Linq;
using Zenject;
using TMPro;

public class DPSMeter : MonoBehaviour
{
    [SerializeField] int seconds = 10;

    [HideInInspector] ReactiveProperty<float> dps = new();

    [Inject] void Construct(UnityEvent<DamageArgs> damageEvent, TextMeshProUGUI dpsLabel)
    {
        damageEvent.AsObservable()
            .Select(args => args.damage)
            .Buffer(TimeSpan.FromSeconds(1))
            .Select(argsInSecond =>
                    argsInSecond.DefaultIfEmpty(0).Sum())
            .TakeLast(seconds)
            .ToList()
            .Subscribe(d =>
            {
                dps.Value = d.Sum() / d.Count;
            })
            .AddTo(this);
    }
}
