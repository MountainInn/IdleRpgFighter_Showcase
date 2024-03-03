using System.Linq;
using UnityEngine;
using Zenject;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        InstantiateSOs<Talent>("SO/Talents");
    }

    void InstantiateSOs<T>(string path)
        where T : ScriptableObject
    {
        var objects = Resources.LoadAll<T>(path);
        objects
            .Select(t => Instantiate(t))
            .Map(Container.Inject);
    }

    override public void InstallBindings()
    {
        Container
            .Bind(
                typeof(Mob),
                typeof(Character),
                typeof(Vault)
                // typeof(MobSpawner),
                // typeof(Battle),
                // typeof(Corridor),
                // typeof(HitAnimation)
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
