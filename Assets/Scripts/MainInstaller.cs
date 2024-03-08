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
    [SerializeField] CollectionAnimation prefabDropable;
    [Space]
    [SerializeField] ParticleSystem onPickupPS;

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

    new void Start()
    {
        base.Start();

        InstantiateSOs<Talent>("SO/Talents/");
    }

    override public void InstallBindings()
    {
        Container
            .Bind(
                typeof(Mob),
                typeof(Character),
                typeof(Vault),
                typeof(LootManager),
                // typeof(Battle),
                // typeof(Corridor),
                typeof(Arena)
            )
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind(typeof(Combatant), typeof(AnimatorCombatant))
            .To<Mob>()
            .FromResolve()
            .WhenInjectedInto<Character>();

        Container
            .Bind(typeof(Combatant), typeof(AnimatorCombatant))
            .To<Character>()
            .FromResolve()
            .WhenInjectedInto<Mob>();

        Container
            .Bind(typeof(DamageModifier), typeof(IInitializable))
            .To(t => t.AllTypes().DerivingFrom<DamageModifier>())
            .AsTransient()
            .NonLazy();

        Container
            .Bind<ParticleSystem>()
            .FromMethod(() => onPickupPS)
            .WhenInjectedInto<CollectionAnimation>();

        Container
            .BindMemoryPool<CollectionAnimation, CollectionAnimation.Pool>()
            .FromComponentInNewPrefab(prefabDropable)
            .UnderTransformGroup("[Dropables]");

        Container
            .Bind<CollectionAnimation>()
            .FromNewComponentOnNewPrefab(prefabDropable)
            .AsTransient();
    }
}
