using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class LootManager : MonoBehaviour
{
    DropParticlesConfig dropParticlesConfig;

    [Inject] Character character;
    [Inject] CollectionAnimation.Pool dropablesPool;
    [Inject] GameSettings gameSettings;
    [Inject] Vault vault;
    [Inject] DiContainer Container;

    public void Subscribe(Combatant combatant)
    {
        combatant.onDie
            .AsObservable()
            .Subscribe(_ =>
            {
                Drops(combatant);
            })
            .AddTo(combatant);

        dropParticlesConfig = Container.Resolve<DropParticlesConfig>();
        dropParticlesConfig.fields .Sort((a, b) => b.goldAmount.CompareTo(a.goldAmount));
        dropParticlesConfig = Instantiate(dropParticlesConfig);

        foreach (var field in dropParticlesConfig.fields)
        {
            field.lootParticles =
                Container
                .InstantiatePrefabForComponent<LootParticles>(field.lootParticles);

            field.lootParticles.transform.position =
                combatant.transform.position + new Vector3(0, 0.5f, 0);
           
            field.lootParticles
                .onParticleHitCharacter
                .AddListener(() => LootGold(field.goldAmount));
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
        int a = amount;

        DropParticlesConfig.Field cacheField = default;

        foreach (var field in dropParticlesConfig.fields)
        {
            cacheField = field;

            while (a > 0 &&
                   a > field.goldAmount)
            {
                a -= field.goldAmount;

                field.lootParticles.Emit();
            }
        }

        if (a > 0)
        {
            a -= cacheField.goldAmount;

            cacheField.lootParticles.Emit();

            goldMargin = a;
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
