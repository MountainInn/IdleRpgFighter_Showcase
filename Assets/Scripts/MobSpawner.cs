using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using UnityEngine.Pool;
using Zenject;
using System;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] MobQueue mobQueue;

    [Inject] Mob mob;
    [Inject]
    public void Construct()
    {
        StartCoroutine( Mobs() );
    }

    public IEnumerator Mobs()
    {
        foreach(var so in mobQueue.Next())
        {
            Debug.Log($"Random: {so.name}");

            mob.SetStats(so);

            yield return
                mob.afterDeathAnimation
                .AsObservable()
                .Take(1)
                .ToYieldInstruction();

            Debug.Log($"After Death");
        }
    }

}
