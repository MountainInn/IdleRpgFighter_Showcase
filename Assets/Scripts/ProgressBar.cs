
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Sprite borderSprite;
    [SerializeField] Sprite maskSprite;
    [SerializeField] Sprite fillSprite;
    [SerializeField] float underFillDelay;
    [Space]
    [SerializeField] [Range(0,1f)] float fillAmount;
    [SerializeField] float pixelsPerUnit;
    [SerializeField] Color borderColor;
    [SerializeField] Color fillColor;
    [Space]
    [SerializeField] Image borderImage;
    [SerializeField] Image maskImage;
    [SerializeField] Image fillImage;
    [Space]
    [SerializeField] TextMeshProUGUI label;
    [Header("Afterimage")]
    [SerializeField] ProgressBar afterimage;
    [SerializeField] Sprite afterimageSprite;
    [SerializeField] Color afterimageColor;

    Slider slider;

    Queue<Tween> queue = new();

    void OnValidate()
    {
        slider = GetComponent<Slider>();

        if (slider)
        {
            slider.value = fillAmount;
        }


        if (afterimage)
        {
            afterimage.fillSprite = afterimageSprite;
            afterimage.pixelsPerUnit = pixelsPerUnit;
            afterimage.fillColor = afterimageColor;
            afterimage.borderColor = borderColor;
            afterimage.fillAmount = fillAmount + .1f;
            afterimage.maskSprite = maskSprite;
            afterimage.borderSprite = borderSprite;

            afterimage.OnValidate();
        }

        if (fillImage)
        {
            fillImage.sprite = fillSprite;
            fillImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Sliced;
        }

        if (maskImage)
        {
            maskImage.sprite = maskSprite;
            maskImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            maskImage.type = Image.Type.Sliced;
        }

        if (borderImage)
        {
            borderImage.sprite = borderSprite;
            borderImage.pixelsPerUnitMultiplier = pixelsPerUnit;
            borderImage.color = borderColor;
            borderImage.type = Image.Type.Sliced;
        }
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

                slider.value = tup.ratio;

                var tween = afterimage.slider.DOValue(tup.ratio, underFillDelay);

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
