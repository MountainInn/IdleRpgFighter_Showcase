using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

abstract public class Combatant : MonoBehaviour
{
    [SerializeField] public Volume health;
    [Space]
    [SerializeField] protected StatsSO stats;
    [SerializeField] public List<Weapon> weapons;
    [SerializeField] protected LayerMask targetLayers;
    [Space]
    [SerializeField] public UnityEvent onDie;
    [SerializeField] public UnityEvent<Combatant> onKill;
    [HideInInspector] public int defense;

    [Inject] DiContainer container;

    public StatsSO Stats => stats;
    public LayerMask TargetLayers => targetLayers;

    public Action<DamageArgs>
        preAttack,
        preTakeDamage,
        postTakeDamage,
        postAttack;

    protected void Awake()
    {
        stats = Instantiate(stats);
       
        health = new Volume(stats.health);

        weapons =
            weapons
            .Select(prefabWeapon =>
                    container.InstantiatePrefabForComponent<Weapon>(prefabWeapon,
                                                                    transform))
            .ToList();

        weapons ??= new();
    }

    protected void OnEnable()
    {
        health.Refill();
    }

    public Weapon EquipWeapon(Weapon prefabWeapon)
    {
        Weapon instantiatedWeapon =
            container
            .InstantiatePrefabForComponent<Weapon>(prefabWeapon,
                                                   transform);
        weapons.Add(instantiatedWeapon);

        return instantiatedWeapon;
    }

    public void RemoveWeapon(Weapon prefabWeapon)
    {
        Weapon firstSimilarWeapon =
            weapons.First(w => w.stats == prefabWeapon.stats);

        weapons.Remove(firstSimilarWeapon);

        GameObject.Destroy(firstSimilarWeapon.gameObject);
    }

    public void InflictDamage(Combatant defender, float damage)
    {
        DamageArgs args = new DamageArgs()
        {
            attacker = this,
            defender = defender,
            damage = damage
        };

        preAttack?.Invoke(args);

        defender.TakeDamage(args);

        postAttack?.Invoke(args);

        if (!defender.IsAlive)
        {
            onKill?.Invoke(defender);

            defender.onDie?.Invoke();
        }
    }

    public void TakeDamage(DamageArgs args)
    {       
        preTakeDamage?.Invoke(args);

        health.Subtract(args.damage);

        postTakeDamage?.Invoke(args);
    }

    public bool IsAlive => health.current.Value > 0;
}
