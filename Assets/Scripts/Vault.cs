using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Vault : MonoBehaviour
{
    static public Vault instance => _inst ??= FindObjectOfType<Vault>();
    static Vault _inst;

    [SerializeField] public Currency souls;

    [Inject]
    public void Construct(Character character)
    {
        // character.onKill.AddListener((combatant) =>
        // {
        //     if (combatant is Mob mob)
        //         souls.value.Value += mob.MobStats.soulReward;
        // });
    }
    void Awake()
    {
        souls.value.Value = 0;
    }
}
