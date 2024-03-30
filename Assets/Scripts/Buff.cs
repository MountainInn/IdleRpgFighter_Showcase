using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Linq;

public abstract class Buff
{
    public float duration;
    public float multiplier;

    protected float activeBonus = 1f;

    abstract public void Subscribe(Combatant combatant);

    public void StartBuff(GameObject holder)
    {
        Observable
            .Timer(TimeSpan.FromSeconds(duration))
            .DoOnSubscribe( ActivateBonus )
            .DoOnCompleted( DeactivateBonus )
            .Subscribe()
            .AddTo(holder);
    }

    protected virtual void ActivateBonus()
    {
        activeBonus = multiplier;
    }

    protected virtual void DeactivateBonus()
    {
        activeBonus = 1f;
    }
}
