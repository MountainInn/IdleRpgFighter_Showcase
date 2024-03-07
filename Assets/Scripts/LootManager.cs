using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;

public class LootManager : MonoBehaviour
{
    [Inject] Character character;
    [Inject]
    public void Construct(Mob mob, CollectionAnimation.Pool dropablesPool)
    {
        mob.onDie.AddListener(() =>
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
        });
    }

    void Loot(Drop drop)
    {
        if (drop.currency != null)
        {
            drop.currency.GetPaid();
        }
    }

}
