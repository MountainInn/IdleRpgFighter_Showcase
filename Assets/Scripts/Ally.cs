using Zenject;
using UniRx;
using UnityEngine;

public class Ally : AnimatorCombatant
{
    protected MobStatsSO mobStats;
    public MobStatsSO MobStats => mobStats;

    protected bool mobCanAttack;

    protected void Awake()
    {
        SubscribeCanAttack();
        SubscribeToAttackTimerFull();
    }

    public new void SetStats(StatsSO stats)
    {
        attackTimer.ResetToZero();
        attackTimer.Resize(stats.attackTimer);
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

    public class Pool : MonoMemoryPool<Ally>
    {
       
    }
}
