using UnityEngine;
using TMPro;
using Zenject;
using DG.Tweening;
using UnityEngine.Events;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] public UnityEvent onEndTween;

    float duration;
    Vector3 velocity;
    RectTransform rect => transform as RectTransform;
    Vector3 endPosition => rect.anchoredPosition3D + velocity * duration;

    void OnValidate()
    {
        Debug.DrawLine(rect.anchoredPosition3D, endPosition, Color.green, 1);
    }

    public void SetColor(Color color)
    {
        label.color = color;
    }

    public void StartTween()
    {
        DOTween.Sequence()
            .Join(rect
                  .DOAnchorPos(endPosition, duration)
                  .SetEase(Ease.OutBack, 2))
            .Join(label
                  .DOColor(new Color(.3f,.3f,.3f,0f), duration)
                  .SetEase(Ease.InOutQuint))
            .OnKill(onEndTween.Invoke);
    }

    public class Pool : MonoMemoryPool<string, Vector3, float, Vector3, FloatingText>
    {
        protected override void OnCreated(FloatingText item)
        {
            base.OnCreated(item);
            item.onEndTween.AddListener(() => Despawn(item));
        }

        protected override void Reinitialize(string text,
                                             Vector3 startingPosition,
                                             float duration,
                                             Vector3 velocity,
                                             FloatingText item)
        {
            item.transform.position = startingPosition;
            item.label.text = text;
            item.label.alpha = 1;
            item.duration = duration;
            item.velocity = velocity;
        }
    }
}
