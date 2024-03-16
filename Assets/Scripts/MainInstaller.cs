using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class MainInstaller : MonoInstaller
{
    [SerializeField] BlockVfx blockVfx;
    [SerializeField] AttackBonusVfx attackBonusVfx;
    [Space]
    [SerializeField] Transform levelHolder;

    [Inject] Ally prefabAlly;


    List<T> InstantiateSOs<T>(string path)
        where T : ScriptableObject
    {
        var objects = Resources.LoadAll<T>(path);

        return
            objects
            .Select(t => Instantiate(t))
            .Map(Container.Inject)
            .ToList();
    }

    override public void InstallBindings()
    {
        Container
            .Bind(
                typeof(Mob),
                typeof(Arena),
                typeof(LevelSwitcher),
                typeof(SaveSystemUser),
                typeof(CharacterSpawnPoint)
            )
            .FromComponentInHierarchy()
            .AsSingle();

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

        Container
            .Bind<Transform>()
            .FromInstance(levelHolder)
            .WhenInjectedInto<LevelSwitcher>();
    }
}
