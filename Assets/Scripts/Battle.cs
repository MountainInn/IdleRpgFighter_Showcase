using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using System.Linq;
using System.Collections;
using System;

public class Battle : MonoBehaviour
{
    [Inject] Canvas canvas;
    [Inject] Character character;
    [Inject] Mob mob;
    [Inject]
    public void SubscribeWeakPoints(TalentUser talentUser,
                                    WeakPointView.Pool weakPointViewPool)
    {
        WeakPoints weakPoints =
            talentUser
            .talents
            .OfType<WeakPoints>()
            .Single();

        Observable
            .Interval(weakPoints.rollInterval)
            .Subscribe(_ =>
            {
                weakPoints.Roll(canvas, weakPointViewPool);
            })
            .AddTo(canvas);

        weakPointViewPool.onWeakPointClicked = () =>
        {
            weakPoints.Shoot(mob, character);
        };
    }
}
