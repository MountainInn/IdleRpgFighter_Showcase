using UnityEngine;
using System;
using Zenject;
using UniRx;

[CreateAssetMenu(fileName = "RectuitAlly", menuName = "SO/Talents/RectuitAlly")]
public class RecruitAlly : Talent
{
    [SerializeField] MobStatsSO allyStats;

    [Inject] Ally.Pool allyPool;
    [Inject] Gang gang;
    [Inject]
    public void Construct()
    {

    }

    protected override void OnLevelUp(int level, Price price)
    {
        var newAlly = allyPool.Spawn();

        allyStats.Apply(newAlly);

        gang.Add(newAlly);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("*BLANK*");
    }
}
