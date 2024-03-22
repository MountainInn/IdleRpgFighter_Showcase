using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class GulagInstaller : BaseInstaller
{
    [SerializeField] ProgressBar chargeProgressBar;

    override public void InstallBindings()
    {
        BindSpawnPoint();

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

        Container.Bind<Combatant>().To<Rock>().FromResolve().WhenInjectedInto<Character>();

        Container.Bind<ProgressBar>().FromInstance(chargeProgressBar).AsSingle();
    }
}
