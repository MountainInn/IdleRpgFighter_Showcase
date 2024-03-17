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
            .MaybeRegister<int>(this,
                                "gold",
                                () => gold.value.Value,
                                (val) => gold.value.Value = val);
    }

    void Awake()
    {
        gold.value.Value = 0;
    }
}
