using UnityEngine;
using Zenject;

static public class DiContainerExtension
{

    public static ConditionCopyNonLazyBinder BindView<T>(this DiContainer Container, T prefabView, Transform parent)
        where T : Component
    {
        return
            Container
            .Bind<T>()
            .FromComponentInNewPrefab(prefabView)
            .AsTransient()
            .OnInstantiated<T>((ctx, view) =>
            {
                view.transform.SetParent(parent);
                view.transform.localScale = Vector3.one;
            });
    }
}
