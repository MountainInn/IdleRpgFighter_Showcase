using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Zenject;

public class CharacterController : MonoBehaviour
{
    [Inject] Button attackButton;
    [Inject] Character character;

    void Awake()
    {
        attackButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                character.EnterAttackState();
            })
            .AddTo(this);
    }
}
