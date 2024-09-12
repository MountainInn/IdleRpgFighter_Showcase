using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Warrior : Character
{
    public Character target;
    public float AttackPower
    {
        get { return attackPower;}
        set { attackPower = value; }
    }
    [SerializeField] float attackPower;

    public UnityEvent OnAttack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        OnAttack.Invoke();
    }
}
