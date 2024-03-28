using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public abstract class BaseInstaller : MonoInstaller
{
    [SerializeField] protected RuntimeAnimatorController characterAnimatorController;
    [Space]
    [SerializeField] protected Canvas canvas;
    [Space]
    [SerializeField] protected CharacterSpawnPoint characterSpawnPoint;
    [SerializeField] protected ParticleSystemForceField particleSystemForce;
    [Space]
    [SerializeField] FloatingText prefabFloatingText;
    [SerializeField] CritFloatingText prefabCritFloatingText;
    [Space]
    [SerializeField] FloatingTextSpawner mobDamagedFloatingText;

    override public void InstallBindings()
    {
        Container
            .Bind<Canvas>()
            .FromInstance(canvas)
            .AsCached();

        Container
            .Bind<CharacterSpawnPoint>()
            .FromInstance(characterSpawnPoint)
            .AsSingle();

        Container
            .Bind<ParticleSystemForceField>()
            .FromInstance(particleSystemForce)
            .AsSingle();

        Container
            .Bind<NominalParticles>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<FloatingTextSpawner>()
            .FromMethod(_ => mobDamagedFloatingText)
            .AsSingle()
            .WhenInjectedInto(typeof(Mob), typeof(Rock));

        Container
            .BindMemoryPool<FloatingText, FloatingText.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(prefabFloatingText)
            .UnderTransform(canvas.transform);

        Container
            .BindMemoryPool<CritFloatingText, CritFloatingText.Pool>()
            .WithInitialSize(3)
            .FromComponentInNewPrefab(prefabCritFloatingText)
            .UnderTransform(canvas.transform);

    }

    protected List<T> InstantiateSOs<T>(string path)
        where T : ScriptableObject
    {
        var objects = Resources.LoadAll<T>(path);

        return
            objects
            .Select(t => Instantiate(t))
            .Map(Container.Inject)
            .ToList();
    }
}
