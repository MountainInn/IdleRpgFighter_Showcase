using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;

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
    [SerializeField] UnityEvent onCharacterReset;
    [SerializeField] UnityEvent onMobReset;
   
    [Inject] Character character;
    [Inject] Mob mob;
  
    [Inject] public void Construct()
    {
        character.onDie.AddListener( SlideCharacter );

        mob.onDie.AddListener( SlideMob );

        onCharacterReset.AddListener(() =>
        {
            character.Respawn();
        });

        onMobReset.AddListener(() =>
        {
            mob.Respawn();
        });
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
            .Insert(hatchOpeningDuration * .8f,
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

        characterRoot
            .DOLocalMove(Vector3.zero, returnDuration)
            .OnKill( onCharacterReset.Invoke );
    }

    void ResetMob()
    {
        mobRoot.position = mobRespawn.position;

        mobRoot
            .DOLocalMove(Vector3.zero, returnDuration)
            .OnKill( onMobReset.Invoke );
    }
}
