using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

// TODO: changed mesh to prefab later
public class RockEffectHandler : MonoBehaviour
{
    [SerializeField] Transform _rock, _rockSpawn;
    [SerializeField] ParticleSystem _fallPS,_destroyPS;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] float _fallTime =2f;
    [SerializeField] List<Mesh> _awailableMeshes;
    bool _particleTriggered;
    [ContextMenu("TestKill")]
    public void CrushRock()
    {
        _meshFilter.gameObject.SetActive(false);
        _destroyPS.Play();
    }
    [ContextMenu("TestSpawn")]
    public void SpawnRock()
    {
        _meshFilter.gameObject.SetActive(true);
        _particleTriggered = false;
        _meshFilter.sharedMesh = _awailableMeshes.GetRandom();
        _rock.position = _rockSpawn.position;
        _rock.DOLocalMove(Vector3.zero, _fallTime).SetEase(Ease.OutBounce).OnUpdate(TrySpawnParticle);
    }

    private void TrySpawnParticle()
    {
        if (_particleTriggered) return;
        if (_rock.transform.localPosition.y > 0.1f) return;
        _fallPS.Play();
        _particleTriggered = true;
    }
}
