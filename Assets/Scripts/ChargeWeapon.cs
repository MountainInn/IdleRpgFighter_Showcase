using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class ChargeWeapon : Weapon
{
    protected override void Subscribe()
    {
        base.Subscribe();

        // ObservePreparationOngoing()
        //     .Subscribe(isPreparing => canTick = isPreparing)
        //     .AddTo(this);

        owner.attackTimer.ObserveFull()
            .Subscribe(isFull =>
            {
                // if (isFull)
                //     preparationEnd.Invoke();
            })
            .AddTo(this);
    }

    protected override Combatant SelectTarget(IEnumerable<Combatant> aliveTargets)
    {
        return base.SelectTarget(aliveTargets);
    }

    protected override void Attack(Combatant target)
    {
        attackStart.Invoke();
    }

    protected override IEnumerable<IObservable<bool>> GetCanAttackObservables()
    {
        return
            base.GetCanAttackObservables()
            // .Append( ObservePreparationNotOngoing() )
            .Append( owner.health.ObserveEmpty().Select(e => !e) );
    }
}
