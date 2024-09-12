using UnityEngine;
using UniRx;
using Zenject;

public class AttackInput : MonoBehaviour
{
    [Inject]
    public void Construct(Character character,
                          RuntimeAnimatorController attackAnimatorController)
    {
        character.SetAnimatorController(attackAnimatorController);
    }
}
