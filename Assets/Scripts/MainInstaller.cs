using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class MainInstaller : BaseInstaller
{
    [Space]
    [SerializeField] BlockVfx blockVfx;
    [SerializeField] AttackBonusVfx attackBonusVfx;
    [Space]
    [SerializeField] WeakPointView prefabWeakPoint;

    [Inject] Ally prefabAlly;



    override public void InstallBindings()
    {
        Container
            .Bind(
                typeof(Mob),
                typeof(Arena),
                typeof(AttackInput)
            )
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<CharacterSpawnPoint>()
            .FromInstance(characterSpawnPoint)
            .AsSingle();

        Container
            .BindMemoryPool<WeakPointView, WeakPointView.Pool>()
            .FromComponentInNewPrefab(prefabWeakPoint)
            .UnderTransform(canvasTransform);

        Container
            .Bind<RuntimeAnimatorController>()
            .FromInstance(characterAnimatorController)
            .WhenInjectedInto<AttackInput>();

        Container
            .Bind<List<Talent>>()
            .FromMethod(() => InstantiateSOs<Talent>("SO/Talents/"))
            .AsSingle();

        Container
            .Bind<List<Ability>>()
            .FromMethod(() => InstantiateSOs<Ability>("SO/Abilities/"))
            .AsSingle();

        Container
            .Bind<TalentUser>()
            .FromMethod(() =>
            {
                var users =
                    GameObject
                    .FindObjectsOfType<TalentUser>();

                var injected = users.FirstOrDefault(u => u.alreadyInjected);

                if (injected != null)
                    return injected;
                else
                {
                    var user =
                        new GameObject(nameof(TalentUser))
                        .AddComponent<TalentUser>();

                    Container.Inject(user);

                    DontDestroyOnLoad(user);
                    user.alreadyInjected = true;

                    return user;
                }
            })
            .AsSingle()
            .NonLazy();

        Container
            .BindMemoryPool<Ally, Ally.Pool>()
            .FromComponentInNewPrefab( prefabAlly )
            .AsTransient();


        Container
            .Bind(typeof(Combatant), typeof(AnimatorCombatant))
            .To<Mob>()
            .FromResolve()
            .WhenInjectedInto(typeof(Character), typeof(Ally));

        Container
            .Bind(typeof(DamageModifier), typeof(IInitializable))
            .To(t => t.AllTypes().DerivingFrom<DamageModifier>())
            .AsTransient()
            .NonLazy();

        Container .Bind<BlockVfx>() .FromInstance(blockVfx);
        Container .Bind<AttackBonusVfx>() .FromInstance(attackBonusVfx);
    }
}
