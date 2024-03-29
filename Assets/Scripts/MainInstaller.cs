using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class MainInstaller : BaseInstaller
{
    [Space]
    [SerializeField] BlockVfx blockVfx;
    [SerializeField] AttackBonusVfx attackBonusVfx;
    [Space]
    [SerializeField] WeakPointView prefabWeakPoint;
    [Space]
    [SerializeField] SegmentedProgressBar arenaProgressBar;

    [Inject] Ally prefabAlly;

    override public void InstallBindings()
    {
        base.InstallBindings();
       
        Container
            .Bind(
                typeof(Mob),
                typeof(Arena),
                typeof(Battle),
                typeof(AttackInput)
            )
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<SegmentedProgressBar>()
            .FromMethod(() => arenaProgressBar)
            .WhenInjectedInto<Journey>();
       
        Container
            .BindMemoryPool<WeakPointView, WeakPointView.Pool>()
            .FromComponentInNewPrefab(prefabWeakPoint)
            .UnderTransform(canvas.transform);

        Container
            .Bind<RuntimeAnimatorController>()
            .FromInstance(characterAnimatorController)
            .WhenInjectedInto<AttackInput>();

        Container
            .BindMemoryPool<Ally, Ally.Pool>()
            .FromComponentInNewPrefab( prefabAlly )
            .AsTransient();


        Container
            .Bind(typeof(Combatant), typeof(AnimatorCombatant))
            .To<Mob>()
            .FromResolve()
            .WhenInjectedInto(typeof(Character), typeof(Ally));

        Container
            .Bind(typeof(DamageModifier), typeof(IInitializable))
            .To(t => t.AllTypes().DerivingFrom<DamageModifier>())
            .AsTransient()
            .NonLazy();

        Container .Bind<BlockVfx>() .FromInstance(blockVfx);
        Container .Bind<AttackBonusVfx>() .FromInstance(attackBonusVfx);
    }
}
