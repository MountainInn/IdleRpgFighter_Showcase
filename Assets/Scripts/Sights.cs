using UnityEngine;
using UniRx;
using System;
using System.Linq;


[RequireComponent(typeof(CircleCollider2D))]
public class Sights<T> : MonoBehaviour
    where T : Component
{
    [SerializeField] CircleCollider2D _collider;

    public CircleCollider2D Collider { get => _collider ??= GetComponent<CircleCollider2D>(); }
    public ReactiveCollection<T> targets {get; protected set;} = new();

    public IObservable<bool> ObserveHasTargets()
    {
        return targets .ObserveCountChanged(true) .Select(count => count > 0);
    }

    public void SetLayers(LayerMask excludeLayers, LayerMask includeLayers)
    {
        Collider.excludeLayers = excludeLayers;
        Collider.includeLayers = includeLayers;
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out T component)
            && component != GetComponentInParent<T>())
        {
            targets.Add(component);
        }
    }

    protected void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out T component)
            && component != GetComponentInParent<T>())
        {
            targets.Remove(component);
        }
    }
}
