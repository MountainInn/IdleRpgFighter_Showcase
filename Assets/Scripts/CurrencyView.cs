using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class CurrencyView : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMPro.TextMeshProUGUI label;

    CompositeDisposable disposables = new();

    void OnValidate()
    {
        if (icon == null)
        {
            icon = new GameObject("Icon").AddComponent<Image>();
            icon.transform.SetParent(transform);
        }

        if (label == null)
        {
            label = new GameObject("Label").AddComponent<TMPro.TextMeshProUGUI>();
            label.transform.SetParent(transform);
        }
    }

    public void Clear()
    {
        icon.sprite = null;
        label.text = "";
    }

    public void ClearDisposables()
    {
        disposables.Clear();
        disposables = new();
    }

    public void InitAndSubscribe(Currency currency)
    {
        Init(currency);

        currency.amount
            .Subscribe(v => label.text = currency.amount.Value.ToString())
            .AddTo(disposables);
    }

    public void Init(Currency currency)
    {
        icon.sprite = currency.sprite;
        label.text = currency.amount.Value.ToString();
    }

    public void InitAndSubscribe(Price price)
    {
        Init(price);

        price.cost
            .Subscribe(c => label.text = c.ToString())
            .AddTo(disposables);

        price
            .IsAffordableObservable()
            .Subscribe(aff =>
                       label.color = (aff) ? Color.green : Color.red)
            .AddTo(disposables);
    }

    public void Init(Price price)
    {
        icon.sprite = price.currency.sprite;
        label.text = price.cost.Value.ToString();
    }
}
