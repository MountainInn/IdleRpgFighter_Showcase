using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

public class CharacterController : MonoBehaviour
{
    [SerializeField] Button attackButton;
    [SerializeField] Character character;

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
