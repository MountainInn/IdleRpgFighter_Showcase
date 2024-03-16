using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;

public class LootManager : MonoBehaviour
{
    [Inject] Character character;
    [Inject] CollectionAnimation.Pool dropablesPool;

    public void Subscribe(Combatant combatant)
    {
        combatant.onDie
            .AsObservable()
            .Subscribe(_ =>
            {
                combatant.dropList
                    ?.entries
                    .Where(entry => (UnityEngine.Random.value < entry.chance))
                    ?.Map(entry =>
                    {
                        CollectionAnimation dropable = dropablesPool.Spawn();

                        dropable.transform.position = combatant.transform.position;

                        dropable.oneShotOnPickup += () =>
                        {
                            Loot(entry.drop);
                        };

                        dropable.StartCollectionAnimation(character.transform);
                    });
            })
            .AddTo(combatant);
    }

    void Loot(Drop drop)
    {
        drop.currency?.GetPaid();
    }
}
