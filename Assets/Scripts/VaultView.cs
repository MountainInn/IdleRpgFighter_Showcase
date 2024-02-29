using UnityEngine;
using Zenject;
using System;
using UniRx;

public class VaultView : MonoBehaviour
{
    [SerializeField] CurrencyView soulsView;
    [SerializeField] FloatingTextSpawner floatingTextSpawner;

    [Inject] public void Construct(Vault vault)
    {
        soulsView.InitAndSubscribe(vault.souls);

        vault.souls
            .ObserveChange()
            .Subscribe(change =>
            {                
                floatingTextSpawner.Float(change.ToString());
            })
            .AddTo(this);
    }
}
