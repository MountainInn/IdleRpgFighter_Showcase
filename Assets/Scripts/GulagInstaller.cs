using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class GulagInstaller : MonoInstaller
{
    [SerializeField] Button attackButton;
    [Space]
    [SerializeField] ParticleSystem onPickupPS;
    [Space]
    [SerializeField] MobView rockMobView;
    [SerializeField] ProgressBar chargeProgressBar;
    [SerializeField] FloatingTextSpawner mobDamagedFloatingText;
    [SerializeField] Transform canvasTransform;

    override public void InstallBindings()
    {
        Container
            .Bind<ParticleSystem>()
            .FromInstance(onPickupPS)
            .WhenInjectedInto<CollectionAnimation>();


        Container
            .Bind<Button>()
            .FromInstance(attackButton)
            .WhenInjectedInto<CharacterController>();

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
