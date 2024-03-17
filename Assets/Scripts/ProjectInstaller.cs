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

    override public void InstallBindings()
    {
        Container
            .Bind(
                typeof(Vault),
                typeof(LootManager),
                typeof(Gang),
                typeof(DPSMeter),
                typeof(SceneLoader),
                typeof(FullScreenCover)
            )
            .FromComponentsInHierarchy()
            .AsSingle();

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
