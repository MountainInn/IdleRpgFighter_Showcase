using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class DPSMeter : MonoBehaviour
{
    [SerializeField] int seconds = 10;

    Queue<float> damageSecondsQueue = new();

    public IObservable<float> ObserveDPS(Mob mob)
    {
        return mob.postTakeDamage.AsObservable()
            .Select(args => args.damage)
            .Buffer(TimeSpan.FromSeconds(1))
            .Select(damagesInSecond =>
            {
                float sum = damagesInSecond.DefaultIfEmpty(0).Sum();

                float dps = CalculateDPS(sum);

                return dps;
            })
            .StartWith(0);
    }

    float CalculateDPS(float damageSecond)
    {
        int count = damageSecondsQueue.Count;

        if (count >= seconds)
            damageSecondsQueue.Dequeue();

        damageSecondsQueue.Enqueue(damageSecond);

        count += 1;

        float dps = damageSecondsQueue.Sum() / count;
        return dps;
    }
}
