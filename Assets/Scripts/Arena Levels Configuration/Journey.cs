using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using Zenject;

public class Journey : MonoBehaviour
{
    [SerializeField] JourneySO journeySO;

    [SerializeField] public UnityEvent onSegmentCompleted;
    [SerializeField] public UnityEvent onQueueCompleted;
    [SerializeField] public UnityEvent onJourneyCompleted;

    SuperVolume arenaProgress;

    SaveState saveState;
    IEnumerable<IEnumerable<MobStatsSO>> queue;

    public struct SaveState
    {
        public int mobStatIndex;
        public int segmentIndex;
        public int queueIndex;
        public string journeyName;

        public SaveState(int mobStatIndex, int segmentIndex, int queueIndex, string journeyName)
        {
            this.mobStatIndex = mobStatIndex;
            this.segmentIndex = segmentIndex;
            this.queueIndex = queueIndex;
            this.journeyName = journeyName;
        }
    }

    CompositeDisposable subscriptions;
    Coroutine queueCoroutine;

    [Inject] Mob mob;
    [Inject] Arena arena;
    [Inject] SegmentedProgressBar arenaProgressBar;
    [Inject] LevelSwitcher LevelSwitcher;
    [Inject] SaveSystem saveSystem;
    [Inject]
    void RegisterWithSaveSystem(SaveSystem saveSystem)
    {
        saveSystem
            .MaybeRegister<SaveState>(this,
                                      "journeyState",
                                      () => saveState,
                                      (val) => saveState = val,
                                      () =>
                                      {
                                          StartQueue();
                                      });
    }

    void Start()
    {
        if (queueCoroutine == null)
            StartQueue();
    }

    void ResetQueuePosition()
    {
        saveState.mobStatIndex = 0;
        saveState.segmentIndex = 0;
        saveState.queueIndex = 0;
    }

    void SubscribeArenaProgress(MobQueue mobQueue)
    {
        subscriptions?.Dispose();
        subscriptions = new();

        mobQueue.GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths, out int totalLength);

        arenaProgress = new SuperVolume(subLengths);

        arenaProgress
            .ObserveSubvolumeFull()
            .Subscribe(tuple => OnSegmentFinished())
            .AddTo(subscriptions);

        arenaProgressBar
            .Subscribe(queue, arenaProgress, subscriptions);
    }

    void OnSegmentFinished()
    {
        onSegmentCompleted?.Invoke();
    }

    public void StartQueue()
    {
        queueCoroutine = StartCoroutine( Mobs() );
    }

    IEnumerator Mobs()
    {

        foreach (var (q, journeyField) in journeySO.queues.Enumerate())
        {
            saveState.queueIndex = q;

            yield return
                LevelSwitcher
                .MaybeSwitchLevel(journeyField.levelSceneBuildIndex);

            MobQueue mobQueue = journeyField.mobQueue;
            mobQueue.GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths,
                                                 out int totalLength);

            queue = mobQueue.GenerateQueue();

            SubscribeArenaProgress(mobQueue);

            foreach (var (s, segment) in queue.Enumerate())
            {
                saveState.segmentIndex = s;
                foreach (var (m, mobStat) in segment.Enumerate())
                {
                    saveState.mobStatIndex = m;

                    mobStat.Apply(mob);

                    yield return mob.onDie.TakeYield(1);

                    arenaProgress.Add(1);

                    yield return arena.onMobMovedToRespawnPosition.TakeYield(1);
                }
            }

            journeyField.onQueueCompleted?.Invoke();
            onQueueCompleted?.Invoke();
        }

        queueCoroutine = null;

        StartQueue();
    }

    public void LoadSaveData(SaveState saveState)
    {
        StopAllCoroutines();

        this.saveState = saveState;

        journeySO = Resources.Load<JourneySO>($"SO/Journeys/{saveState.journeyName}");
    }

    public SaveState GetSaveData()
    {
        return this.saveState;
    }
}
