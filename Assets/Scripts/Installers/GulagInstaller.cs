using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class GulagInstaller : BaseInstaller
{
    [SerializeField] ProgressBar chargeProgressBar;
    [Space]
    [SerializeField] Button attackButton;

    override public void InstallBindings()
    {
        base.InstallBindings();

        Container
            .Bind<RuntimeAnimatorController>()
            .FromInstance(characterAnimatorController)
            .WhenInjectedInto<PickaxeInput>();

        Container.Bind<Combatant>().FromInstance(null).WhenInjectedInto<Rock>();

        Container
            .Bind(
                typeof(PickaxeInput),
                typeof(Rock)
            )
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<CharacterController>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<Button>()
            .FromMethod(() => attackButton)
            .WhenInjectedInto<CharacterController>();
        Container.Bind<Combatant>().To<Rock>().FromResolve().WhenInjectedInto<Character>();

        Container.Bind<ProgressBar>().FromInstance(chargeProgressBar).AsSingle();
    }
}
