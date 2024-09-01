using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RockEffectHandler : MonoBehaviour
{
    public UnityEvent ONRockReady;
    [SerializeField] Transform _rock, _rockSpawn;
    [SerializeField] Animation _anim;
    [SerializeField] ParticleSystem _fallPS,_destroyPS;
    [SerializeField] float _fallTime =2f;
    [SerializeField] List<GameObject> _objectPrefs;
    List<GameObject> _awailableObjs;
    GameObject _curObject;
    private void Awake()
    {
        _awailableObjs = new(_objectPrefs.Count);
           
        foreach (var obj in _objectPrefs)
        {
            var go = Instantiate(obj);
            go.transform.SetParent(_rock,false);
            go.SetActive(false);
            _awailableObjs.Add(go);
        }
    }
    [ContextMenu("TestKill")]
    public void CrushRock()
    {
        _curObject.gameObject.SetActive(false);
        _awailableObjs.Add(_curObject);
        _destroyPS.Play();
    }
    [ContextMenu("TestSpawn")]
    public void SpawnRock()
    {
        _curObject = _awailableObjs.GetRandom();
        _curObject.gameObject.SetActive(true);
        _rock.position = _rockSpawn.position;
        //_rock.DOLocalMove(Vector3.zero, _fallTime).SetEase(Ease.InExpo).OnUpdate(TrySpawnParticle);
        _anim.Play();
    }

    private void SpawnParticle()
    {
        _fallPS.Play();
    }
    private void RockReady()
    {
        ONRockReady?.Invoke();
    }
}
