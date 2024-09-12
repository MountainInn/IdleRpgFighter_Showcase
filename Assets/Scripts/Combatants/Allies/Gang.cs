using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using System;

public class Gang : MonoBehaviour
{
    List<Ally> allies = new();

    [Inject] RecruitAlly recruitAlly;
    [Inject] Ally.Pool allyPool;
    [Inject]
    public void Construct(AllySpawnPoint allySpawnPoint)
    {
        recruitAlly
            .ObserveAllies()
            .Subscribe(allyStats =>
            {
                Ally ally = allyPool.Spawn();

                allyStats.Apply(ally);

                allySpawnPoint.ApplyPosition(ally.transform);

                allies.Add(ally);
            })
            .AddTo(this);
    }

    void OnDisable()
    {
        foreach (var item in allies)
        {
            if (item)
                allyPool.Despawn(item);
        }
    }
}
