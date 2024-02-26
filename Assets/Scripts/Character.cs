using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    public float Health
    {
        get { return health;}
        set
        {
            if (value > 0)
            {
                health = value;
            }
            else
            {
                health = 0;
                Death();
            }
        }
    }
    [SerializeField] float health;
    public UnityEvent OnDeath;
    public void Death()
    {
        OnDeath.Invoke();
    }
}
