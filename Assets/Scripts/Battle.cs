using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using System.Linq;
using System.Collections;
using System;

public class Battle : MonoBehaviour
{
    [SerializeField] public UnityEvent onBattleStarted;
    [SerializeField] public UnityEvent onBattleWon;
    [SerializeField] public UnityEvent onBattleLost;

    [Inject] Character character;
    [Inject] ProgressBar mobHealthView;

    public ReactiveProperty<Mob> battleTarget = new(null);
    public bool IsOngoing => battleTarget.Value != null;

    void Start()
    {
        mobHealthView.gameObject.SetActive(false);

        character.onFoundMob.AsObservable()
            .Subscribe(mob => StartBattle(mob))
            .AddTo(this);
    }

    void StartBattle(Mob mob)
    {
        mobHealthView.Subscribe(mob.gameObject, mob.health);
        mobHealthView.gameObject.SetActive(true);

        mob.onDie
            .AsObservable()
            .Take(1)
            .Subscribe(_ =>
            {
                mobHealthView.gameObject.SetActive(false);

                onBattleWon?.Invoke();
            })
            .AddTo(this);

        character.onDie
            .AsObservable()
            .Take(1)
            .Subscribe(_ => onBattleLost?.Invoke())
            .AddTo(this);

        battleTarget.Value = mob;

        StartCoroutine(BattleCoroutine());

        onBattleStarted?.Invoke();

        IEnumerator BattleCoroutine()
        {
            var combatants = new Combatant[] { character, mob };

            while (character.IsAlive && mob.IsAlive)
            {
                if (character.AttackTimerTick(Time.deltaTime))
                {
                }

                if (mob.AttackTimerTick(Time.deltaTime))
                {
                }

                yield return null;
            }

            battleTarget.Value = null;
        }
    }
}
