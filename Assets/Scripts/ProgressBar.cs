
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] [Range(0,1f)] float fillAmount;
    [SerializeField] float pixelsPerUnit;
    [SerializeField] Color fillColor;
    [SerializeField] Color underFillColor;
    [SerializeField] Color underImageColor;
    [Space]
    [SerializeField] Image underImage;
    [Space]
    [SerializeField] Image fillMask;
    [SerializeField] Image fillImage;
    [Space]
    [SerializeField] Image underFillMask;
    [SerializeField] Image underFillImage;
    [SerializeField] float underFillDelay;
    [Space]
    [SerializeField] TextMeshProUGUI label;

    Queue<Tween> queue = new();

    void OnValidate()
    {
        if (fillImage)
        {
            fillImage.sprite = sprite;
            fillImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            fillImage.color = fillColor;
        }

        if (underFillImage)
        {
            underFillImage.sprite = sprite;
            underFillImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            underFillImage.color = underFillColor;
        }

        if (underImage)
        {
            underImage.sprite = sprite;
            underImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            underImage.color = underImageColor;
        }

        if (fillMask)
            fillMask.fillAmount = fillAmount;

        if (underFillMask)
            underFillMask.fillAmount = fillAmount + .1f;
    }

    public void Subscribe(GameObject volumeOwner, Volume volume)
    {
        volume
            .ObserveAll()
            .TakeWhile(_ => volumeOwner.activeSelf)
            .Subscribe(tup =>
            {
                if (label)
                    label.text = volume.ToString();

                fillMask.fillAmount = tup.ratio;

                var tween = underFillMask.DOFillAmount(tup.ratio, underFillDelay);

                QueueTween(tween);
            })
            .AddTo(volumeOwner);
    }

    void QueueTween(Tween tween)
    {
        queue.Enqueue(tween);

        tween.OnKill(() =>
        {
            queue.Dequeue();

            queue.Peek()?.Play();
        });

        if (queue.Peek() == tween)
        {
            tween.Play();
        }
    }
}
