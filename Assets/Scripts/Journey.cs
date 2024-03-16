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

    [Inject] Mob mob;
    [Inject] Arena arena;
    [Inject] SegmentedProgressBar arenaProgressBar;
    [Inject] LevelSwitcher LevelSwitcher;
    [Inject] SaveSystem saveSystem;
    [Inject]
    void RegisterWithSaveSystem(SaveSystem saveSystem)
    {
        saveSystem
            .MaybeRegister<SaveState>("journeyState",
                                      () => saveState,
                                      (val) => saveState = val,
                                      () =>
                                      {
                                          Debug.Log($"Start Queue After Load");
                                          StartQueue();
                                      },
                                      this);
    }

    void ResetQueuePosition()
    {
        saveState.mobStatIndex = 0;
        saveState.segmentIndex = 0;
        saveState.queueIndex = 0;
    }

    void SubscribeArenaProgress(IEnumerable<int> subLengths)
    {
        subscriptions?.Dispose();
        subscriptions = new();

        arenaProgress = new SuperVolume(subLengths);

        arenaProgress
            .ObserveSubvolumeFull()
            .Subscribe(tuple => OnSegmentFinished())
            .AddTo(subscriptions);

        arenaProgressBar
            .Subscribe(arenaProgress, subscriptions);
    }

    void OnSegmentFinished()
    {
        onSegmentCompleted?.Invoke();
    }

    public void StartQueue()
    {
        StartCoroutine( Mobs() );
    }

    IEnumerator Mobs()
    {
        for (; saveState.queueIndex < journeySO.queues.Count; saveState.queueIndex++)
        {
            JourneySO.Field journeyField = journeySO.queues[saveState.queueIndex];

            yield return
                LevelSwitcher
                .MaybeSwitchLevel(journeyField.levelPrefab);

            MobQueue mobQueue = journeyField.mobQueue;
            mobQueue.GetSubLengthsAndTotalLength(out IEnumerable<int> subLengths,
                                                 out int totalLength);

            SubscribeArenaProgress(subLengths);


            IEnumerable<IEnumerable<MobStatsSO>> queue = mobQueue.GenerateQueue();

            for (; saveState.segmentIndex < queue.Count(); saveState.segmentIndex++)
            {
                IEnumerable<MobStatsSO> segment = queue.ElementAt(saveState.segmentIndex);

                for (; saveState.mobStatIndex < segment.Count(); saveState.mobStatIndex++)
                {
                    MobStatsSO so = segment.ElementAt(saveState.mobStatIndex);

                    so.Apply(mob);

                    yield return mob.onDie.TakeYield(1);

                    arenaProgress.Add(1);

                    yield return arena.onMobMovedToRespawnPosition.TakeYield(1);
                }
            }

            journeyField.onQueueCompleted?.Invoke();
            onQueueCompleted?.Invoke();
        }
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
