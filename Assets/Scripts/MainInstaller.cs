using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class MainInstaller : MonoInstaller
{
    [Inject] Ally prefabAlly;

    new void Start()
    {
        base.Start();

        InstantiateSOs<Talent>("SO/Talents/");
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
                typeof(Arena)
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
    }
}
