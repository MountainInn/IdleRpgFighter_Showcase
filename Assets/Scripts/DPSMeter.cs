using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using System.Linq;
using Zenject;
using TMPro;
using System.Collections.Generic;

public class DPSMeter : MonoBehaviour
{
    [SerializeField] int seconds = 10;

    Queue<float> damageSecondsQueue = new();
    TextMeshProUGUI dpsLabel;

    void Awake()
    {
        SetText(0);
    }

    [Inject] void Construct(Mob mob, TextMeshProUGUI dpsLabel)
    {
        this.dpsLabel = dpsLabel;
       
        mob.postTakeDamage.AsObservable()
            .Select(args => args.damage)
            .Buffer(TimeSpan.FromSeconds(1))
            .Select(damagesInSecond =>
                    damagesInSecond.DefaultIfEmpty(0).Sum())
            .Subscribe(damageSecond =>
            {
                int count = damageSecondsQueue.Count;

                if (count >= seconds)
                    damageSecondsQueue.Dequeue();

                damageSecondsQueue.Enqueue(damageSecond);

                count += 1;

                float dps = damageSecondsQueue.Sum() / count;

                SetText(dps);
            })
            .AddTo(this);
    }

    void SetText(float dps)
    {
        dpsLabel.text = $"DPS: {dps:F0}";
    }
}
