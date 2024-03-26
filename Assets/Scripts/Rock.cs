using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class Rock : Combatant
{
    [SerializeField] float minHealth = 30;
    [SerializeField] float maxHealth = 130;
    [SerializeField] float crushTweenDuration = .3f;
    [SerializeField] private RockEffectHandler effectHandler;

    Vector3 originalScale, squishScale;

    [Inject]
    public void Construct(FloatingTextSpawner floatingTextSpawner, MobView mobView)
    {
        postTakeDamage.AsObservable()
            .Subscribe(args =>
            {
                floatingTextSpawner.FloatDamage(args);

                DoSquish();
            })
            .AddTo(this);
    }

    void Awake()
    {
        originalScale = transform.lossyScale;
        squishScale = originalScale * 0.8f;

        Respawn();
    }

    Sequence squish;

    Sequence DoSquish()
    {
        if (squish != null)
        {
            squish.Kill();

            transform.localScale = originalScale;
        }

        return
            squish = DOTween
            .Sequence()
            .Join(transform
                  .DOPunchScale(squishScale, .15f)
            );
    }

    [Inject] void SubToView(MobView mobView)
    {
        mobView.Subscribe(this);

        var fade = mobView.GetComponent<Fade>();

        onDie.AddListener(() => fade.FadeOut());
        onRespawn.AddListener(() => fade.FadeIn());
    }

    [Inject] void SubscribeToCharacter(Character character)
    {
        character.SetTarget(this);
    }

    [Inject] void SubscribeToDPSMeter(DPSMeter dpsMeter, DPSMeterView dpsView)
    {
        dpsMeter
            .ObserveDPS(this)
            .Subscribe(dpsView.SetText)
            .AddTo(this);
    }

    [Inject] void SubscribeToLootManager(LootManager lootManager)
    {
        lootManager.Subscribe(this);
    }


    void Start()
    {
        health
            .ObserveEmpty()
            .WhereEqual(true)
            .Subscribe(async _ => await OnCrushed())
            .AddTo(this);
    }

    async UniTask OnCrushed()
    {
        // await squish?.AsyncWaitForKill();
        effectHandler.CrushRock();

        await UniTask.WaitForSeconds(crushTweenDuration);
        Respawn();
    }

    public new void Respawn()
    {
        float newMaximum = UnityEngine.Random.Range(minHealth, maxHealth);
        health.ResizeAndRefill(newMaximum);
        effectHandler.SpawnRock();
        onRespawn?.Invoke();
    }
}
