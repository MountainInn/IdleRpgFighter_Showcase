using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

public class Battle : MonoBehaviour
{
    [Inject] Canvas canvas;
    [Inject] Character character;
    [Inject] Mob mob;

    [Inject]
    public void Construct(DiContainer Container,
                          List<Ability> abilities)
    {
        abilities
            .Map(a =>
            {
                var abilityButton = Container.Resolve<AbilityButton>();
                a.SubscribeButton(character, abilityButton);
            });
    }
    [Inject]
    public void SubscribeBlock(Block block, BlockVfx blockVfx, AttackBonusVfx attackBonusVfx)
    {
        block.Subscribe(blockVfx, attackBonusVfx);
    }

    [Inject]
    public void SubscribeWeakPoints(WeakPoints weakPoints,
                                    WeakPointView.Pool weakPointViewPool)
    {
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
