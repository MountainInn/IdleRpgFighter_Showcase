using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] bool visible;
    [SerializeField] bool interactable;
    [SerializeField] float duration;

    void Awake()
    {
        if (visible)
            FadeIn();
        else
            FadeOut();
    }

    public void Toggle()
    {
        if (visible)
            FadeOut();
        else
            FadeIn();
    }

    public void FadeIn()
    {
        canvasGroup.DOKill();

        canvasGroup
            .DOFade(1, duration)
            .SetEase(Ease.OutQuad)
            .OnKill(() =>
            {
                visible = true;

                ToggleInteractable();
            });
    }

    public void FadeOut()
    {
        canvasGroup.DOKill();

        canvasGroup
            .DOFade(0, duration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                visible = false;

                ToggleInteractable();
            });
    }

    void ToggleInteractable()
    {
canvasGroup.blocksRaycasts =
                    canvasGroup.interactable = (visible && interactable);
    }
}
