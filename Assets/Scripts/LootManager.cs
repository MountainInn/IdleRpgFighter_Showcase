using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    NominalParticles nominalParticles;

    [Inject] Character character;
    [Inject] GameSettings gameSettings;
    [Inject] Vault vault;
    [Inject] DiContainer Container;

    public void Subscribe(Combatant combatant, NominalParticles nominalParticles)
    {
        combatant.onDie
            .AsObservable()
            .Subscribe(_ =>
            {
                Drops(combatant);
            })
            .AddTo(combatant);

        this.nominalParticles = nominalParticles;

        foreach (var field in nominalParticles.Fields)
        {
            field.particles.transform.position =
                combatant.transform.position + new Vector3(0, 0.5f, 0);
           
            field.particles
                .onParticleHitCharacter
                .AddListener(() => LootGold(field.amount));
        }
    }

    async UniTask Drops(Combatant combatant)
    {
        if (combatant.dropList != null)
        {
            foreach (var entry in combatant.dropList.entries)
            {
                if (UnityEngine.Random.value < entry.chance)
                {
                    Price currency = entry.drop.currency;

                    if (currency != null)
                    {
                        DropGold(currency.cost.Value);
                    }

                    await UniTask.WaitForSeconds(gameSettings.intervalBetweenDrops);
                }
            }
        }
    }

    int goldMargin;

    void DropGold(int amount)
    {
        var nominals =
            nominalParticles
            .Fields
            .Select(f => f.amount);

        var res = Recurse(amount, nominals, 0);



        List<int> Recurse(int amount, IEnumerable<int> nominals, int totalQuantity)
        {
            List<int> res = new();

            int nominal = nominals.First();

            int maxQuantity = amount / nominal;

            (maxQuantity + 1)
                .ToRange().Shuffle()
                .Map(quantity =>
                {
                    List<int> res = new();

                    int nextAmount = amount - quantity * nominal;

                    int nextTotalQuantity = totalQuantity + quantity;

                    if (nextTotalQuantity > gameSettings.maxParticleCount)
                        return;

                    res.Add(quantity);

                    if (nominals.Count() == 1)
                    {
                        res.Log();
                        return;
                    }

                    res.AddRange(
                        Recurse(nextAmount,
                                nominals.Skip(1),
                                totalQuantity + quantity) );
                });

            return res;
        }
    }

    void LootGold(int amount)
    {
        if (goldMargin != 0)
        {
            amount += goldMargin;
            goldMargin = 0;
        }

        vault.gold.value.Value += amount;
    }
}
