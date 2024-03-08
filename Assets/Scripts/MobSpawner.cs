using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using UnityEngine.Pool;
using Zenject;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] MobQueue mobQueue;
    [SerializeField] public UnityEvent onSegmentFinished;

    SuperVolume arenaProgress;
    IEnumerable<IEnumerable<MobStatsSO>> queue;

    CompositeDisposable subscriptions = new();

    [Inject] Mob mob;
    [Inject] Arena arena;
    [Inject] SegmentedProgressBar arenaProgressBar;
    [Inject]
    public void Construct()
    {
        InitializeNextQueue();

    }

    void InitializeNextQueue()
    {
        queue = mobQueue.GenerateQueue();

        mobQueue.GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths,
                                             out int totalLength);


        arenaProgress = new SuperVolume(subLengths);

        subscriptions?.Dispose();
        subscriptions = new();

        arenaProgressBar
            .Subscribe(arenaProgress, subscriptions);

        arenaProgress
            .ObserveSubvolumeFull()
            .Subscribe(tuple => OnSegmentFinished())
            .AddTo(this);


        StartCoroutine( Mobs() );
    }

    void OnSegmentFinished()
    {
        onSegmentFinished?.Invoke();
    }
    
    IEnumerator Mobs()
    {
        foreach (var segment in queue)
        {
            foreach(var so in segment)
            {
                mob.SetStats(so);

                yield return
                    mob.onDie
                    .AsObservable()
                    .Take(1)
                    .ToYieldInstruction();

                arenaProgress.Add(1);

                yield return
                    arena.onMobMovedToRespawnPosition
                    .AsObservable()
                    .Take(1)
                    .ToYieldInstruction();
            }
        }
    }
}
