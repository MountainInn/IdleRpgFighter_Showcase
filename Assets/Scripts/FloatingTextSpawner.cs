using UnityEngine;
using Zenject;
using UniRx;
using System;

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

    [Inject] void SubscribeToBattle(Battle battle)
    {
        battle.onBattleStarted.AddListener(() => active = 0);
    }

    public void Float(string text)
    {
        int angle = (active - halfMaxCount) * anglePerActive;

        Vector3 privateVelocity =
            Quaternion.Euler(0, 0, angle) * velocity;

        Vector3 privateOffset =
            (active - halfMaxCount) * offset
            + UnityEngine.Random.onUnitSphere;

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
}
