using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using UniRx;

public class Vault : MonoBehaviour
{
    static public Vault instance => _inst ??= FindObjectOfType<Vault>();
    static Vault _inst;

    [SerializeField] public Currency gold;

    [Inject]
    public void Construct(Character character, VaultView view)
    {
        view.goldView.InitAndSubscribe(gold);

        if (view.floatingTextSpawner)
            gold
                .ObserveChange()
                .Subscribe(change =>
                {
                    view.floatingTextSpawner.Float(change.ToString());
                })
                .AddTo(this);
        // character.onKill.AddListener((combatant) =>
        // {
        //     if (combatant is Mob mob)
        //         souls.value.Value += mob.MobStats.soulReward;
        // });
    }

    void Awake()
    {
        gold.value.Value = 0;
    }
}
