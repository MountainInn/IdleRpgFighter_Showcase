using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;
using System.Collections;
using DG.Tweening;

public partial class Mob : Combatant
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] public UnityEvent onAskedToReturnToPool;
    [SerializeField] public UnityEvent onEnterPreparationState;
    [SerializeField] public UnityEvent onExitPreparationState;

    MobStatsSO mobStats;

    Color baseColor;
    bool mobCanAttack;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;

    [Inject] public void Construct(Character character)
    {
        base.Construct();
       
        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(attackTriggerId))
            .AddTo(this);

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);


        mobCanAttack = true;
        onDie.AddListener(() => mobCanAttack = false);
        onRespawn.AddListener(() => mobCanAttack = true);
    }

    [Inject] void SubToView(MobView mobView)
    {
        mobView.Subscribe(this);

        var fade = mobView.GetComponent<Fade>();

        afterDeathAnimation.AddListener(fade.FadeOut);
        onRespawn.AddListener(fade.FadeIn);
    }

    [Inject] void SubToDropables(Character character, CollectionAnimation.Pool dropablesPool)
    {
        onDie.AddListener(() =>
        {
            mobStats.dropList.entries
                .Where(entry => (UnityEngine.Random.value < entry.chance))
                ?.Map(entry =>
                {
                    CollectionAnimation dropable = dropablesPool.Spawn();

                    dropable.transform.position = transform.position;

                    dropable.oneShotOnPickup += () =>
                    {
                        character.Loot(entry);
                    };

                    dropable.StartCollectionAnimation(character.transform);
                });
        });
    }

    public void SetStats(MobStatsSO mobStats)
    {
        base.SetStats(mobStats);

        this.mobStats = (MobStatsSO)this.Stats;

        mobStats.template.ApplyTemplate(gameObject);
    }

    void Update()
    {
        if (mobCanAttack && CanContinueBattle())
            AttackTimerTick(Time.deltaTime);
    }

    public void ReturnToPool()
    {
        onAskedToReturnToPool.Invoke();
    }
}
