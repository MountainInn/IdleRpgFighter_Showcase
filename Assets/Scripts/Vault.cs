using UnityEngine;
using Zenject;

public class Vault : MonoBehaviour
{
    static public Vault instance => _inst ??= FindObjectOfType<Vault>();
    static Vault _inst;

    [SerializeField] public Currency gold;

    [Inject] SaveSystem saveSystem;
    [Inject]
    void RegisterWithSaveSystem()
    {
        saveSystem
            .MaybeRegister("gold",
                      () => gold.value.Value,
                      (val) => gold.value.Value = val.GetAs<int>());
    }

    void Awake()
    {
        gold.value.Value = 0;
    }
}
