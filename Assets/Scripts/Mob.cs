using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using System;
using System.Collections;
using DG.Tweening;

public partial class Mob : Combatant
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] public UnityEvent onDied;
    [SerializeField] public UnityEvent onAskedToReturnToPool;
    [Space]
    [Header("Mob")]
    [SerializeField] StatsSO mobStats;
    [SerializeField] float dyingDuration;
    [SerializeField] int shiverLoops = 6;
    [SerializeField] float gettingHitDuration;

    public StatsSO MobStats => mobStats;

    int baseMapProp = Shader.PropertyToID("_BaseMap");
    ParticleSystem ps;

    Color baseColor;

    [Inject] DiContainer Container;

    [Inject]
    public void Construct(StatsSO mobStatSO,
                          Character character, FloatingTextSpawner floatingTextSpawner)
    {
        // base.Construct(mobStats);

        // this.mobStats = (StatsSO)this.stats;

        // meshRenderer.material.SetTexture(baseMapProp, mobStatSO.texture);

        // ps = Container
        //     .InstantiatePrefabForComponent<ParticleSystem>(mobStatSO.attackEffectSystem,
        //                                                    transform);
        // ps.transform.localPosition = Vector3.zero;

        // ps
        //     .GetComponent<ProjectileParticles>()
        //     .onParticleHitCharacter
        //     .AddListener(() => Attack(character));

        // attackTimer.ObserveFull()
        //     .WhereEqual(true)
        //     .Subscribe(_ => ps.Emit(1))
        //     .AddTo(this);

        // baseColor = meshRenderer.material.color;

        // postTakeDamage.AsObservable()
        //     .SelectMany( GettingHit )
        //     .Subscribe()
        //     .AddTo(this);

        // onDie.AsObservable()
        //     .SelectMany( Dying )
        //     .Subscribe(_ => onDied?.Invoke())
        //     .AddTo(this);

        // this.floatingTextSpawner = floatingTextSpawner;
    }

    public void ReturnToPool()
    {
        onAskedToReturnToPool.Invoke();
    }
}

