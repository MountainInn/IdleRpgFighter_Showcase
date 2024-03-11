using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;
using System;
using UniRx;

public class Arena : MonoBehaviour
{
    [SerializeField] float slideDistance;
    [SerializeField] float slideDuration;
    [Space]
    [SerializeField] float openHatchAngle;
    [SerializeField] float hatchOpeningDuration;
    [SerializeField] float hatchClosingDuration;
    [Space]
    [SerializeField] float returnDuration;
    [Space]
    [SerializeField] Transform mobRoot;
    [SerializeField] Transform characterRoot;
    [SerializeField] Transform mobRespawn, mobHatch;
    [SerializeField] Transform characterRespawn, characterHatch;
    [Space]
    [SerializeField] public UnityEvent onCharacterMovedToRespawnPosition;
    [SerializeField] public UnityEvent onCharacterReset;
    [SerializeField] public UnityEvent onMobMovedToRespawnPosition;
    [SerializeField] public UnityEvent onMobReset;
   
    [Inject] Character character;
    [Inject] Mob mob;
  
    void Start()
    {
        characterRoot = character.transform;

        characterRoot.SetParent(characterHatch);
        characterRoot.GetLocalPositionAndRotation(out Vector3 localPosition,
                                                  out Quaternion localRotation);
        localPosition = Vector3.zero;
        localPosition.y = mobRoot.position.y;

        characterRoot.SetPositionAndRotation(localPosition, localRotation);

        character.afterDeathAnimation
            .AddListener( SlideCharacter );

        mob.afterDeathAnimation
            .AddListener( SlideMob );

        onCharacterReset
            .AddListener( character.Respawn );

        onMobReset
            .AddListener( mob.Respawn );
    }

    public void SlideCharacter()
    {
        DoSlide(characterRoot, characterHatch, slideDistance, openHatchAngle)
            .OnKill( ResetCharacter );
    }

    public void SlideMob()
    {
        DoSlide(mobRoot, mobHatch, -slideDistance, -openHatchAngle)
            .OnKill( ResetMob );
    }

    Tween DoSlide(Transform combatant, Transform hatch, float endZ, float endAngle)
    {
        return
            DOTween
            .Sequence()
            .Append(hatch
                    .DORotate(new Vector3(endAngle, 0, 0), hatchOpeningDuration)
            )
            .Insert(hatchOpeningDuration * .4f,
                    combatant
                    .DOLocalMoveZ(combatant.position.z + endZ, slideDuration)
                    .SetEase(Ease.InQuad)
            )
            .Append(hatch
                    .DORotate(new Vector3(0, 0, 0), hatchClosingDuration)
            );
    }

    void ResetCharacter()
    {
        characterRoot.position = characterRespawn.position;

        onCharacterMovedToRespawnPosition?.Invoke();

        character.combatantAnimator.SetTrigger("respawn");

        characterRoot
            .DOLocalMove(Vector3.zero, returnDuration)
            .OnKill( onCharacterReset.Invoke );
    }

    void ResetMob()
    {
        mobRoot.position = mobRespawn.position;

        onMobMovedToRespawnPosition?.Invoke();

        mob.combatantAnimator.SetTrigger("respawn");

        mobRoot
            .DOLocalMove(Vector3.zero, returnDuration)
            .OnKill( onMobReset.Invoke );
    }
}
