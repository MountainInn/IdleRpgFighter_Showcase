using UnityEngine;
using Zenject;

public class VaultView : MonoBehaviour
{
    [SerializeField] CurrencyView goldView;

    [Inject]
    public void Construct(Vault vault)
    {
        goldView.InitAndSubscribe(vault.gold);
    }
}
