using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Fade))]
public class WeakPointView : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] Button button;
    [SerializeField] public UnityEvent onWeakPointClicked;

    void Awake()
    {
        button.onClick.AddListener(() =>
        {
            fade.FadeOut();
            onWeakPointClicked?.Invoke();
        });
    }

    void OnEnable()
    {
        fade.FadeIn();
    }

    public class Pool : MonoMemoryPool<WeakPointView>
    {
        public Action onWeakPointClicked;

        protected override void OnCreated(WeakPointView item)
        {
            base.OnCreated(item);

            item.onWeakPointClicked.AddListener(() => this.onWeakPointClicked.Invoke());

            item.fade.onFadeOut.AddListener(() =>
            {
                Despawn(item);
            });
        }
    }
}
