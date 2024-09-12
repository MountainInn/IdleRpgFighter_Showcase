using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class LootParticles : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] public UnityEvent onParticleHitCharacter;

    List<ParticleSystem.Particle> enter = new();

    [Inject]
    void Construct(ParticleSystemForceField collectionField)
    {
        SphereCollider sphereCollider = collectionField.GetComponent<SphereCollider>();

        ps.trigger.AddCollider(sphereCollider);
    }

    public void Emit()
    {
        ParticleSystem.EmitParams emitParams = new ()
        {
            randomSeed = (uint)(UnityEngine.Random.value * 10000),
            applyShapeToPosition = true,
        };

        ps.Emit(emitParams, 1);
    }

    public void OnParticleTrigger()
    {
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < numEnter; i++)
        {
            onParticleHitCharacter.Invoke();
        }
    }
}
