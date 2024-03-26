using TMPro;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [Header("GO prefabs")]
    [SerializeField] CollectionAnimation prefabDropable;
    [SerializeField] Ally prefabAlly;
    [SerializeField] GameObject prefabCharacter;
    [SerializeField] ParticleSystem onPickupPS;
    [SerializeField] Vault vaultInstance;
    [Header("Game Settings")]
    [SerializeField] GameSettings gameSettings;
    [SerializeField] DropParticlesConfig dropParticlesConfig;
    [Space]
    [SerializeField] DPSMeterView dpsMeterView;
    [Space]
    [SerializeField] Transform talentsParent;
    [Space]
    [SerializeField] VaultView vaultView;
    [Space]
    [SerializeField] Transform shopPanel;
    [SerializeField] TalentView talentViewPrefab;
    [Space]
    [SerializeField] Transform abilitiesParent;
    [SerializeField] AbilityButton abilityButtonPrefab;
    [SerializeField] Canvas canvas;


    new void Start()
    {
        base.Start();
        shopPanel.gameObject.SetActive(true);
    }

    override public void InstallBindings()
    {
        BindView(talentViewPrefab, talentsParent);

        BindView(abilityButtonPrefab, abilitiesParent);

        Container
            .Bind<VaultView>()
            .FromMethod(() => vaultView);

        Container
            .Bind(
                typeof(Vault),
                typeof(LootManager),
                typeof(Gang),
                typeof(DPSMeter),
                typeof(LevelSwitcher),
                typeof(MobView),
                typeof(FullScreenCover)
            )
            .FromComponentsInHierarchy()
            .AsSingle();

        Container .Bind<DPSMeterView>() .FromInstance(dpsMeterView);


        Container.Bind<DropParticlesConfig>() .FromInstance(dropParticlesConfig) .AsSingle();

        Container.Bind<GameSettings>() .FromInstance(gameSettings) .AsSingle();

        Container
            .Bind<SaveSystem>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<SaveSystemUser>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<Character>()
            .FromComponentInNewPrefab(prefabCharacter)
            .AsSingle();

        Container
            .Bind(typeof(Combatant), typeof(AnimatorCombatant))
            .To<Character>()
            .FromResolve()
            .WhenInjectedInto<Mob>();

        Container
            .Bind<Ally>()
            .FromInstance(prefabAlly)
            .WhenInjectedInto<MainInstaller>();

        Container
            .Bind<ParticleSystem>()
            .FromComponentInNewPrefab(onPickupPS)
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

    void BindView<T>(T prefabView, Transform parent)
        where T : Component
    {
        Container
            .Bind<T>()
            .FromComponentInNewPrefab(prefabView)
            .AsTransient()
            .OnInstantiated<T>((ctx, view) =>
            {
                view.transform.SetParent(parent);
                view.transform.localScale = Vector3.one;
            });
    }

}
