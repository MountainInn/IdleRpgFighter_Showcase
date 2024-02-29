using System.Linq;
using UnityEngine;
using Zenject;
using TMPro;

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
    // [Space]
    // [SerializeField] FloatingText prefabFloatingText;
    // [SerializeField] FloatingTextSpawner mobDamagedFloatingText;
    // [SerializeField] Transform canvasTransform;

    // StatsSO[] mobStatSOs;
    // Talent[] talents;

    // void Awake()
    // {
    //     mobStatSOs = Resources.LoadAll<StatsSO>("SO/MobStats");

    //     InstantiateSOs<Talent>("SO/Talents");
    //     InstantiateSOs<Ability>("SO/Abilities");
    // }

    // void InstantiateSOs<T>(string path)
    //     where T : ScriptableObject
    // {
    //     var objects = Resources.LoadAll<T>(path);
    //     objects
    //         .Select(t => Instantiate(t))
    //         .Map(Container.Inject);
    // }

    // override public void InstallBindings()
    // {
    //     Container
    //         .Bind(typeof(Vault),
    //               typeof(Character),
    //               typeof(DungeonGuide),
    //               typeof(MobSpawner),
    //               typeof(Battle),
    //               typeof(Corridor),
    //               typeof(HitAnimation)
    //         )
    //         .FromComponentInHierarchy()
    //         .AsSingle();


    //     BindView(prefabTalentView, talentViewParent);
    //     BindView(prefabAbilityView, talentViewParent);

    //     BindView(prefabAbilityButton, abilityButtonParent);

    //     Container
    //         .Bind<StatsSO>()
    //         .FromMethod(() => mobStatSOs.GetRandom())
    //         .AsTransient();

    //     Container
    //         .Bind<StatsSO>()
    //         .FromMethod(() => characterStats)
    //         .AsSingle()
    //         .WhenInjectedInto<Character>();

    //     Container
    //         .Bind<ProgressBar>()
    //         .FromMethod(() => mobHealthView)
    //         .AsSingle();

    //     Container
    //         .Bind<BoxCollider>()
    //         .FromMethod(() => characterHitBox)
    //         .AsSingle();

    //     Container
    //         .BindMemoryPool<FloatingText, FloatingText.Pool>()
    //         .WithInitialSize(10)
    //         .FromComponentInNewPrefab(prefabFloatingText)
    //         .UnderTransform(canvasTransform);

    //     Container
    //         .Bind<FloatingTextSpawner>()
    //         .FromMethod(_ => mobDamagedFloatingText)
    //         .AsSingle()
    //         .WhenInjectedInto<Mob>();
    // }

    void BindView<T>(T prefabView, Transform parent)
        where T : Component
    {
        Container
            .Bind<T>()
            .FromComponentInNewPrefab(prefabView)
            .AsTransient()
            .OnInstantiated<T>((ctx, view) =>
                               view.transform.SetParent(parent));
    }
}
