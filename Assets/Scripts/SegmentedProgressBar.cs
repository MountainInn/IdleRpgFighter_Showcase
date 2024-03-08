
using System.Linq;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System;
using System.Linq;
using System.Collections.Generic;

public class SegmentedProgressBar : ProgressBar
{
    [Header("Segmented")]
    [SerializeField] RectTransform borderIcon;

    List<RectTransform> icons = new();

    void Awake()
    {
        borderIcon.gameObject.SetActive(false);
    }

    public void Subscribe(SuperVolume superVolume, CompositeDisposable subscriptions)
    {
        base.Subscribe(gameObject, superVolume)
            .AddTo(subscriptions);
       
        float width = maskImage.rectTransform.rect.width;

        float superMax = superVolume.maximum.Value;

        var xPositions =
            superVolume.subvolumes
            .Select(v => v.maximum.Value)
            .Scan((a, b) => a + b)
            .Select(x => x / superMax * width);

        icons =
            icons
            .ResizeDestructive(xPositions.Count(),
                               () =>
                               {
                                   var icon = Instantiate(borderIcon, maskImage.rectTransform);

                                   icon.gameObject.SetActive(true);
                                   // ToggleIcon(icon, false);

                                   return icon;
                               },
                               (icon) => Destroy(icon.gameObject))
            .Zip(xPositions,
                 (icon, x) =>
                 {
                     icon.anchoredPosition = new Vector2(x, 0);
                     return icon;
                 })
            .ToList();

        superVolume.ObserveSubvolumeFull()
            .Subscribe(tuple =>
            {
                ToggleIcon(icons[tuple.index], tuple.isFull);
            })
            .AddTo(subscriptions);
    }

    void ToggleIcon(RectTransform icon, bool toggle)
    {
        icon
            .GetChild(0)
            .gameObject.SetActive(toggle);
    }
}
