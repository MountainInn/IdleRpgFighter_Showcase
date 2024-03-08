using UnityEngine;
using UniRx;
using Zenject;

public class AttackInput : MonoBehaviour
{

    [Inject]
    public void Construct(CharacterController characterController,
                          Character character)
    {
        characterController.AttackButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                character.Attack();
            })
            .AddTo(this);
    }


}
