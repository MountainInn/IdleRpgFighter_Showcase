using System.Collections.Generic;
using UnityEngine;

public class Vault : MonoBehaviour
{
    static public Vault instance => _inst ??= FindObjectOfType<Vault>();
    static Vault _inst;

    [SerializeField] public Currency gold;

    void Awake()
    {
        gold.amount.Value = 0;
    }
}
