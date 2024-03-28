using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;

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
        int range = 2;

        foreach (var field in nominalParticles.Fields)
        {
            if (amount <= 0)
            {
                goldMargin = amount;
                break;
            }

            int ceil = Mathf.CeilToInt((float)amount / field.amount);
            int count;

            if (field == nominalParticles.Fields.Last())
            {
                count = ceil;
            }
            else
            {
                int floor = Mathf.Max(0, ceil - range);
                count = UnityEngine.Random.Range(floor, ceil+1);
            }

            count.ForLoop(_ => field.particles.Emit());

            amount -= field.amount * count;
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
