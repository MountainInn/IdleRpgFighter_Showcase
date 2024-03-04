using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CollectionAnimation : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 impuls;
    [SerializeField] float collectionDelay;
    [SerializeField] float moveDuration;
    [SerializeField] UnityAction OnAnimationEnd;

    private void OnValidate()
    {
        rb = rb ?? GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update

    public void StartCollectionAnimation(Transform target)
    {
        Vector3 randomForce = impuls;
        randomForce.x = Random.Range(-1f, 1f) * impuls.x;
        randomForce.z = Random.Range(-1f, 1f) * impuls.z;
        rb.AddForce(randomForce, ForceMode.Impulse);
        this.DelayAction(collectionDelay,()=> MoveToTarget(target));
    }
    private void MoveToTarget(Transform target)
    {
        transform.DOMove(target.position, moveDuration).OnComplete(MoveDone);
    }
    private void MoveDone()
    {
        OnAnimationEnd?.Invoke();
        Destroy(gameObject);
    }
}
