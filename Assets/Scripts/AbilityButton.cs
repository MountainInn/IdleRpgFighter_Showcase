using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;

public class AbilityButton : Button,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI nameLabel;
    [Space]
    [SerializeField] public UnityEvent onPointerDown, onPointerUp, onPointerClick;

    internal void Connect(Ability ability)
    {
        nameLabel = GetComponentInChildren<TextMeshProUGUI>();
        nameLabel.text = ability.name;

        this.UpdateAsObservable()
            .Subscribe(_ => ability.Tick())
            .AddTo(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        onPointerClick?.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        onPointerDown?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        onPointerUp?.Invoke();
    }
}
