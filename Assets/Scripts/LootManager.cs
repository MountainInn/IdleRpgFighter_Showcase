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

    int maxCount = 5;

    void _RecurseDropGold(int amount, int count,
                          int fieldId = 0, List<(int, int)> res = default)
    {
        if (fieldId == dropParticlesConfig.fields.Count ||
            count > maxCount)
        {
            res = null;
            return;
        }

        var field = dropParticlesConfig.fields[fieldId];

        int ceil = Mathf.CeilToInt((float)amount / field.goldAmount);

        ceil
            .ToRange()
            .Shuffle()
            .Map(r =>
            {
                int newCount = count + r;

                _RecurseDropGold(amount, newCount, fieldId+1, res);

            });

        // count += random;

        amount -= count * field.goldAmount;

        res.Add((fieldId, count));

        _RecurseDropGold(amount, fieldId+1, count, res);
    }

    int goldMargin;

    void DropGold(int amount)
    {
        int range = 2;

        foreach (var field in dropParticlesConfig.fields)
        {
            if (amount <= 0)
            {
                goldMargin = amount;
                break;
            }

            int ceil = Mathf.CeilToInt((float)amount / field.goldAmount);
            int count;

            if (field == dropParticlesConfig.fields.Last())
            {
                count = ceil;
            }
            else
            {
                int floor = Mathf.Max(0, ceil - range);
                count = UnityEngine.Random.Range(floor, ceil+1);
            }

            count.ForLoop(_ => field.lootParticles.Emit());

            amount -= field.goldAmount * count;
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
