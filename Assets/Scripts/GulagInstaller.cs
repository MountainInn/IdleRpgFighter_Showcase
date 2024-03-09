using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using UnityEngine.UI;

public class GulagInstaller : MonoInstaller
{
    // [SerializeField] TalentView prefabTalentView;
    // [SerializeField] Transform talentViewParent;
    // [Space]
    // [SerializeField] AbilityView prefabAbilityView;
    // [SerializeField] AbilityButton prefabAbilityButton;
    // [SerializeField] Transform abilityButtonParent;
    // [Space]
    [SerializeField] Button attackButton;
    [SerializeField] CollectionAnimation prefabDropable;
    [Space]
    [SerializeField] ParticleSystem onPickupPS;
    [Space]
    [SerializeField] MobView rockMobView;
    [SerializeField] ProgressBar chargeProgressBar;
    [SerializeField] FloatingText prefabFloatingText;
    [SerializeField] CritFloatingText prefabCritFloatingText;
    [SerializeField] FloatingTextSpawner mobDamagedFloatingText;
    [SerializeField] Transform canvasTransform;

    override public void InstallBindings()
    {
        Container
            .BindMemoryPool<CritFloatingText, CritFloatingText.Pool>()
            .WithInitialSize(3)
            .FromComponentInNewPrefab(prefabCritFloatingText)
            .UnderTransform(canvasTransform);

        Container
            .Bind<ParticleSystem>()
            .FromInstance(onPickupPS)
            .WhenInjectedInto<CollectionAnimation>();

        Container
            .BindMemoryPool<CollectionAnimation, CollectionAnimation.Pool>()
            .FromComponentInNewPrefab(prefabDropable)
            .UnderTransformGroup("[Dropables]");


        Container
            .Bind<Button>()
            .FromInstance(attackButton)
            .WhenInjectedInto<CharacterController>();


        Container
            .BindMemoryPool<FloatingText, FloatingText.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(prefabFloatingText)
            .UnderTransform(canvasTransform);

        Container.Bind<Combatant>().FromInstance(null).WhenInjectedInto<Rock>();

        Container
            .Bind(
                typeof(Character),
                typeof(Vault),
                typeof(LootManager),
                typeof(CharacterController),
                typeof(PickaxeInput),
                // typeof(Corridor),
                // typeof(Arena),
                typeof(Rock)
            )
            .FromComponentInHierarchy()
            .AsSingle();

        Container.Bind<Combatant>().To<Rock>().FromResolve().WhenInjectedInto<Character>();

        Container.Bind<MobView>().FromInstance(rockMobView).AsSingle();

        Container.Bind<ProgressBar>().FromInstance(chargeProgressBar).AsSingle();

        Container.Bind<FloatingTextSpawner>().FromInstance(mobDamagedFloatingText).AsSingle();

        // Container
        //     .Bind(typeof(DamageModifier), typeof(IInitializable))
        //     .To(t => t.AllTypes().DerivingFrom<DamageModifier>())
        //     .AsTransient()
        //     .NonLazy();
    }
}
