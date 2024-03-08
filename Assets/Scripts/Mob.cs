using UnityEngine;
using Zenject;
using UniRx;

public class Mob : AnimatorCombatant
{
    protected MobStatsSO mobStats;
    public MobStatsSO MobStats => mobStats;

    protected bool mobCanAttack;

    [Inject] FloatingTextSpawner takeDamagFloatingTextSpawner;

    protected void Awake()
    {
        SubscribeToAttackTimerFull();

        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);

        SubscribeCanAttack();
    }

    protected void SubscribeToAttackTimerFull()
    {
        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(attackTriggerId))
            .AddTo(this);
    }

    protected void SubscribeCanAttack()
    {
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

    public void SetStats(MobStatsSO mobStats)
    {
        base.SetStats(mobStats);

        this.mobStats = mobStats;
        mobStats.template.ApplyTemplate(gameObject);
    }

    public void Update()
    {
        if (mobCanAttack && CanContinueBattle())
            AttackTimerTick(Time.deltaTime);
    }
}
