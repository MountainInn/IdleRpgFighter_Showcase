using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class CollectionAnimation : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Vector3 impuls;
    [SerializeField] float collectionDelay;
    [SerializeField] float moveDuration;
    [SerializeField] UnityAction OnAnimationEnd;

    [Inject] ParticleSystem onPickupSystem;

    public event Action oneShotOnPickup;

    private const string _TintColor = "_TintColor";

    Color startingColor, endColor;
    Transform target;

    private void OnValidate()
    {
        rb = rb ?? GetComponent<Rigidbody>();
        meshRenderer ??= GetComponent<MeshRenderer>();
        trailRenderer ??= GetComponent<TrailRenderer>();
    }

    void Awake()
    {
        startingColor = meshRenderer.material.GetColor(_TintColor);

        endColor = startingColor;
        endColor.a = 0;
    }

    public void StartCollectionAnimation(Transform target)
    {
        this.target = target;

        Vector3 randomForce = impuls;
        randomForce.x = UnityEngine.Random.Range(-1f, 1f) * impuls.x;
        randomForce.z = UnityEngine.Random.Range(-1f, 1f) * impuls.z;
        rb.AddForce(randomForce, ForceMode.Impulse);
        this.DelayAction(collectionDelay,()=> MoveToTarget(target));
    }
    private void MoveToTarget(Transform target)
    {
        Vector3 targetPosition = target.position;
        targetPosition.x += UnityEngine.Random.Range(-.5f, .5f);
        targetPosition.y += 2;
        targetPosition.z += UnityEngine.Random.Range(-.5f, .5f);

        DOTween
            .Sequence()
            .Join(transform.DOMove(targetPosition, moveDuration))
            .Join(meshRenderer.material.DOColor(endColor, _TintColor, moveDuration))
            .Join(trailRenderer.material.DOColor(endColor, _TintColor, moveDuration))
            .OnKill(MoveDone);
    }
    private void MoveDone()
    {
        OnAnimationEnd?.Invoke();

        onPickupSystem.transform.position = transform.position;
        onPickupSystem.Play();

        oneShotOnPickup?.Invoke();
        oneShotOnPickup = null;
    }

    public class Pool : MonoMemoryPool<CollectionAnimation>
    {
        protected override void OnCreated(CollectionAnimation item)
        {
            base.OnCreated(item);
            item.OnAnimationEnd += () => Despawn(item);
        }

        protected override void Reinitialize(CollectionAnimation item)
        {
            base.Reinitialize(item);

            item.rb.velocity = Vector3.zero;
            item.meshRenderer.material.SetColor(_TintColor, item.startingColor);
            item.trailRenderer.material.SetColor(_TintColor, item.startingColor);
        }
    }
}
