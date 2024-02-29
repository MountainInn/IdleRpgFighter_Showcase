using UnityEngine;

[CreateAssetMenu(fileName = "StatsSO", menuName = "StatsSO")]
public class StatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackDamage;

    public void Add(StatsSO other)
    {
        health += other.health;
        movementSpeed += other.movementSpeed;
        attackSpeed += other.attackSpeed;
        attackRange += other.attackRange;
        attackDamage += other.attackDamage;
    }

    public void Subtract(StatsSO other)
    {
        health -= other.health;
        movementSpeed -= other.movementSpeed;
        attackSpeed -= other.attackSpeed;
        attackRange -= other.attackRange;
        attackDamage -= other.attackDamage;
    }
}
