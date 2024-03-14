using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class MainInstaller : MonoInstaller
{
    [SerializeField] BlockVfx blockVfx;
    [SerializeField] AttackBonusVfx attackBonusVfx;
    [Space]
    [SerializeField] Transform levelHolder;

    [Inject] Ally prefabAlly;

    new void Start()
    {
        base.Start();

        InstantiateSOs<Talent>("SO/Talents/");
        InstantiateSOs<Ability>("SO/Abilities/");
    }
   
    List<T> InstantiateSOs<T>(string path)
        where T : ScriptableObject
    {
        var objects = Resources.LoadAll<T>(path);

        return
            objects
            .Select(t => Instantiate(t))
            .Map(Container.Inject)
            .ToList();
    }

    override public void InstallBindings()
    {
        Container
            .Bind(
                typeof(Mob),
                typeof(Arena),
                typeof(LevelSwitcher)
            )
            .FromComponentInHierarchy()
            .AsSingle();

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

        Container
            .Bind<Transform>()
            .FromInstance(levelHolder)
            .WhenInjectedInto<LevelSwitcher>();
    }
}
