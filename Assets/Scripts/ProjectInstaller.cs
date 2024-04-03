using TMPro;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [Header("GO prefabs")]
    [SerializeField] CollectionAnimation prefabDropable;
    [SerializeField] Ally prefabAlly;
    [SerializeField] GameObject prefabCharacter;
    [Header("Instances")]
    [SerializeField] ParticleSystem onPickupPS;
    [SerializeField] Vault vaultInstance;
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
    [SerializeField] Canvas canvas;
    [Header("Game Settings")]
    [SerializeField] GameSettings gameSettings;
    [SerializeField] Cheats cheats;


    new void Start()
    {
        base.Start();
        shopPanel.gameObject.SetActive(true);
    }

    override public void InstallBindings()
    {
        Container
            .BindView(talentViewPrefab, talentsParent);

        Container
            .Bind<VaultView>()
            .FromMethod(() => vaultView);

        Container
            .Bind(
                typeof(Vault),
                typeof(LootManager),
                typeof(DPSMeter),
                typeof(LevelSwitcher),
                typeof(MobView),
                typeof(TalentUser),
                typeof(FullScreenCover)
            )
            .FromComponentsInHierarchy()
            .AsSingle();

        Container.BindSOs<Talent>("SO/Talents/");
        Container.BindSOs<Ability>("SO/Abilities/");

        Container .Bind<DPSMeterView>() .FromInstance(dpsMeterView);


        Container.Bind<GameSettings>() .FromInstance(gameSettings) .AsSingle();
        Container.Bind<Cheats>() .FromInstance(cheats) .AsSingle();

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
}
