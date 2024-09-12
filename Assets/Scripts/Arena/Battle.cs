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
                          Cheats cheats,
                          List<Ability> abilities)
    {
        abilities
            .Map(a =>
            {
                var abilityButton = Container.Resolve<AbilityButton>();
                a.SubscribeButton(character, abilityButton);
                a.SubscribeToCheats(cheats);
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
        weakPoints.SubscribeSpawnOnTimer(canvas, weakPointViewPool);

        weakPointViewPool.onWeakPointClicked = () =>
        {
            weakPoints.Shoot(mob, character);
        };
    }
}
