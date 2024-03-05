using System.Linq;
using UnityEngine;
using Zenject;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainInstaller : MonoInstaller
{
    // [SerializeField] TalentView prefabTalentView;
    // [SerializeField] Transform talentViewParent;
    // [Space]
    // [SerializeField] AbilityView prefabAbilityView;
    // [SerializeField] AbilityButton prefabAbilityButton;
    // [SerializeField] Transform abilityButtonParent;
    // [Space]
    // [SerializeField] StatsSO characterStats;
    // [SerializeField] ProgressBar mobHealthView;
    // [Space]
    // [SerializeField] BoxCollider characterHitBox;

    // StatsSO[] mobStatSOs;
    // Talent[] talents;

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
            .Bind<List<Talent>>()
            .FromMethod(() => InstantiateSOs<Talent>("SO/Talents/"))
            ;

        Container
            .Bind(
                typeof(Mob),
                typeof(Character),
                typeof(Vault),
                // typeof(MobSpawner),
                // typeof(Battle),
                // typeof(Corridor),
                typeof(Arena)
            )
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<Combatant>().To<Mob>()
            .FromResolve()
            .WhenInjectedInto<Character>();

        Container
            .Bind<Combatant>().To<Character>()
            .FromResolve()
            .WhenInjectedInto<Mob>();

        Container
            .Bind(typeof(DamageModifier), typeof(IInitializable))
            .To(t => t.AllTypes().DerivingFrom<DamageModifier>())
            .AsTransient()
            .NonLazy();
    }
}
