using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public abstract class BaseInstaller : MonoInstaller
{
    [SerializeField] protected RuntimeAnimatorController characterAnimatorController;
    [Space]
    [SerializeField] protected Transform canvasTransform;
    [Space]
    [SerializeField] protected CharacterSpawnPoint characterSpawnPoint;
    [SerializeField] protected ParticleSystemForceField particleSystemForce;

    protected void BindSpawnPoint()
    {
        Container
            .Bind<CharacterSpawnPoint>()
            .FromInstance(characterSpawnPoint)
            .AsSingle();

        Container
            .Bind<ParticleSystemForceField>()
            .FromInstance(particleSystemForce)
            .AsSingle();
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
