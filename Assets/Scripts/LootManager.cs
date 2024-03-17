using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;

public class LootManager : MonoBehaviour
{
    [Inject] Character character;
    [Inject] CollectionAnimation.Pool dropablesPool;
    [Inject]
    public void Construct(GameSettings gameSettings)
    {
        intervalBetweenDrops = gameSettings.intervalBetweenDrops;
    }

    float intervalBetweenDrops;

    public void Subscribe(Combatant combatant)
    {
        combatant.onDie
            .AsObservable()
            .Subscribe(_ =>
            {
                Drops(combatant);
            })
            .AddTo(combatant);
    }

    async UniTask Drops(Combatant combatant)
    {
        if (combatant.dropList != null)
        {
            foreach (var entry in combatant.dropList.entries)
            {
                if (UnityEngine.Random.value < entry.chance)
                {
                    DropEntry(combatant, entry);

                    await UniTask.WaitForSeconds(intervalBetweenDrops);
                }
            }
        }
    }

    private void DropEntry(Combatant combatant, DropList.Entry entry)
    {
        CollectionAnimation dropable = dropablesPool.Spawn();

        dropable.transform.position = combatant.transform.position;

        dropable.oneShotOnPickup += () =>
        {
            Loot(entry.drop);
        };

        dropable.StartCollectionAnimation(character.transform);
    }

    void Loot(Drop drop)
    {
        drop.currency?.GetPaid();
    }
}
