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

    void FadeIn()
    {
        canvasGroup.DOKill();

        canvasGroup
            .DOFade(1, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnKill(() =>
            {
                visible =
                    canvasGroup.blocksRaycasts =
                    canvasGroup.interactable = true;
            });
    }

    void FadeOut()
    {
        canvasGroup.DOKill();

        canvasGroup
            .DOFade(0, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                visible =
                    canvasGroup.blocksRaycasts =
                    canvasGroup.interactable = false;
            });
    }
}
