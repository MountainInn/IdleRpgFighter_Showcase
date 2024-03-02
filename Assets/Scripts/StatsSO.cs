using UnityEngine;

[CreateAssetMenu(fileName = "StatsSO", menuName = "StatsSO")]
public class StatsSO : ScriptableObject
{
    [SerializeField] public float health;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float attackDamage;
}
