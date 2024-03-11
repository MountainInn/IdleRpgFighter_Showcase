using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using UniRx;

public class Vault : MonoBehaviour
{
    static public Vault instance => _inst ??= FindObjectOfType<Vault>();
    static Vault _inst;

    [SerializeField] public Currency gold;

    void Awake()
    {
        gold.value.Value = 0;
    }
}
