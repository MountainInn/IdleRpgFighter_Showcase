using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Linq;

public class FloatingTextSpawner : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 offset;
    [SerializeField] int anglePerActive = -4;
    [SerializeField] int maxCount = 24;

    [Inject] FloatingText.Pool floatingTextPool;

    int active;
    int halfMaxCount => maxCount / 2;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
       
        for (int i = 0; i < maxCount; i++)
        {
            int j = i - halfMaxCount;

            Vector3 privateOffset = GetOffset(j);
            Vector3 privateVelocity = GetVelocity(j);

            Vector3 spawnPos = transform.position + privateOffset;

            Gizmos.DrawLine(spawnPos,
                            spawnPos + privateVelocity * duration);
        }
    }

    // [Inject] void SubscribeToBattle(Battle battle)
    // {
    //     battle.onBattleStarted.AddListener(() => active = 0);
    // }

    public void Float(string text)
    {
        int i = active - halfMaxCount;

        Vector3 privateVelocity = GetVelocity(i);

        Vector3 privateOffset = GetOffset(i);

        var floatingText =
            floatingTextPool.Spawn(text,
                                   transform.position + privateOffset,
                                   duration,
                                   privateVelocity);

        Color textColor = text.Contains('-') ? Color.red : Color.green;

        floatingText.SetColor(textColor);
        floatingText.StartTween();

        active++;
        active %= maxCount;
    }

    private Vector3 GetOffset(int i)
    {
        return i * offset + UnityEngine.Random.onUnitSphere;
    }

    Vector3 GetVelocity(int i)
    {
        int angle = i * anglePerActive;

        return Quaternion.Euler(0, 0, angle) * velocity;
    }
}
