using UnityEngine;
using UniRx;
using System;
using UniRx.Diagnostics;

[CreateAssetMenu(fileName = "GameSettings", menuName = "SO/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Loot Manager")]
    public float intervalBetweenDrops;
    public int maxParticleCount;
    [Header("Save System")]
    public double autoSaveInterval;
    [Header("Gulag")]
    public float gulagDuration;
    [Header("Other")]
    public FloatReactiveProperty globalTimeInterval;

    public void SubscribeToTimer<T>(IDisposable timerSubscription,
                                    Component holder,
                                    IObservable<T> takeUntilStream,
                                    Action onTimer)
    {
        globalTimeInterval
            .Subscribe(t =>
            {
                timerSubscription?.Dispose();

                timerSubscription =
                    Observable
                    .Interval(TimeSpan.FromSeconds(t))
                    .TakeUntil(takeUntilStream)
                    .Subscribe(_ => onTimer.Invoke())
                    .AddTo(holder);
            })
            .AddTo(holder);
    }

        public void SubscribeToTimer(IDisposable timerSubscription,
                                     Component holder,
                                     Action onTimer)
        {
            globalTimeInterval
                .Subscribe(t =>
                {
                    timerSubscription?.Dispose();

                    timerSubscription =
                        Observable
                        .Interval(TimeSpan.FromSeconds(t))
                        .Subscribe(_ => onTimer.Invoke())
                        .AddTo(holder);
                })
                .AddTo(holder);
        }

}
