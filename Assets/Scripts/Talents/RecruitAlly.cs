using UnityEngine;
using System;
using Zenject;
using UniRx;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "RectuitAlly", menuName = "SO/Talents/RectuitAlly")]
public class RecruitAlly : Talent
{
    [SerializeField] public List<MobStatsSO> allyStats;

    [Inject] Cheats cheats;
    [Inject]
    public void Construct()
    {
        buyableLevel.ware.SetLevel(cheats.startingRecruitAllyLevel.Value);
    }

    public IObservable<MobStatsSO> ObserveAllies()
    {
        return
            buyableLevel.ware.level.current
            .WhereNotEqual(0)
            .Select(level => allyStats[(int)level-1]);
    }

    protected override void OnLevelUp(int level, Price price)
    {
        CostUp(level, price);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("*BLANK*");
    }
}
