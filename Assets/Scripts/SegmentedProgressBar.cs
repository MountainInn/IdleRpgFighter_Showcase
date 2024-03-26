
using System.Linq;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class SegmentedProgressBar : ProgressBar
{
    [Header("Segmented")]
    [SerializeField] RectTransform borderIcon;

    List<RectTransform> icons = new();

    void Awake()
    {
        borderIcon.gameObject.SetActive(false);
    }

    public void Subscribe(IEnumerable<IEnumerable<MobStatsSO>> queue, SuperVolume superVolume, CompositeDisposable subscriptions)
    {
        base.Subscribe(gameObject, superVolume)
            .AddTo(subscriptions);
       
        float width = maskImage.rectTransform.rect.width;

        var mobs = queue .SelectMany(segment => segment);

        int mobsCount = mobs.Count();

        var xPositions =
            mobsCount
            .ToRange()
            .Select(i => (float)i / mobsCount * width);


        icons.DestroyAll();

        foreach (var (mob, x) in mobs.Zip(xPositions))
        {
            Image icon =
                Instantiate(borderIcon, maskImage.rectTransform)
                .GetComponent<Image>();

            icon.sprite = mob.icon;
            icon.gameObject.SetActive(true);

            RectTransform rect = icon.transform as RectTransform;

            rect.anchoredPosition = new Vector2(x, 0);

            icons.Add(rect);
        }
    }
}
