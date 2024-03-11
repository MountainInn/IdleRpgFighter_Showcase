using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;

public class LootManager : MonoBehaviour
{
    [Inject] Character character;
    [Inject] CollectionAnimation.Pool dropablesPool;

    public void Subscribe(Mob mob)
    {
        mob.onDie
            .AsObservable()
            .Subscribe(_ =>
            {
                mob.MobStats.dropList
                    .entries
                    .Where(entry => (UnityEngine.Random.value < entry.chance))
                    ?.Map(entry =>
                    {
                        CollectionAnimation dropable = dropablesPool.Spawn();

                        dropable.transform.position = mob.transform.position;

                        dropable.oneShotOnPickup += () =>
                        {
                            Loot(entry.drop);
                        };

                        dropable.StartCollectionAnimation(character.transform);
                    });
            })
            .AddTo(mob);
    }

    void Loot(Drop drop)
    {
        drop.currency?.GetPaid();
    }
}
