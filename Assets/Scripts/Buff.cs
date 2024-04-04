using System;
using UnityEngine;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using Zenject;
using System.Linq;

public class Buff
{
    public float duration;
    public float multiplier;
    public BoolReactiveProperty enabled {get; protected set;} = new();

    protected float activeBonus = 1f;

    IDisposable buffSubscription;

    public IDisposable Subscribe<T>(IObservable<T> stream,
                                    Action<T, float> buffApplication)
    {
        return
            enabled
            .SubToggle(stream,
                       t => buffApplication.Invoke(t, multiplier));
    }

    public IDisposable Subscribe(Action<bool> onToggle)
    {
        return
            enabled
            .Subscribe(onToggle);
    }

    public void StartBuff<T>(T holder)
        where T : Component
    {
        buffSubscription?.Dispose();
       
        buffSubscription =
            Observable
            .Timer(TimeSpan.FromSeconds(duration))
            .DoOnSubscribe( () => enabled.Value = true )
            .DoOnCompleted( () => enabled.Value = false )
            .Debug("Buff")
            .Subscribe()
            .AddTo(holder);
    }
}
