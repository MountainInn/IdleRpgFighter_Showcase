using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;
using System;

public class Arena : MonoBehaviour
{
    [SerializeField] bool dontSwitchScenes;
    [Space]
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
    [Inject] SceneLoader sceneLoader;
  
    void Start()
    {
        characterRoot = character.transform.parent;

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
            .OnKill(() =>
            {
                if (dontSwitchScenes)
                    ResetCharacter();
                else
                    sceneLoader.SwitchToGulag();
            });
    }

    public void SlideMob()
    {
        DoSlide(mobRoot, mobHatch, -slideDistance, -openHatchAngle)
            .OnKill( ResetMob );
    }

    Tween DoSlide(Transform combatant, Transform hatch, float endZ, float endAngle)
    {
        Vector3 position = combatant.position;
        position.z =+ endZ;
        position = Quaternion.AngleAxis(endAngle, Vector3.right) * position;
        return
            DOTween
            .Sequence()
            .Append(hatch
                    .DORotate(new Vector3(endAngle, 0, 0), hatchOpeningDuration)
            )
            .Insert(0f,combatant
                    .DORotate(new Vector3(endAngle, 0, 0), hatchOpeningDuration)
            )
            .Insert(hatchOpeningDuration * .4f,
                    combatant
                    .DOLocalMove(position, slideDuration)
                    .SetEase(Ease.InQuad)
            )
            .Append(hatch
                    .DORotate(new Vector3(0, 0, 0), hatchClosingDuration)
            );
    }

    void ResetCharacter()
    {
        characterRoot.position = characterRespawn.position;
        characterRoot.localRotation = Quaternion.identity;

        onCharacterMovedToRespawnPosition?.Invoke();

        character.combatantAnimator.SetTrigger("respawn");

        characterRoot
            .DOLocalMove(Vector3.zero, returnDuration)
            .OnKill( onCharacterReset.Invoke );
    }

    void ResetMob()
    {
        mobRoot.position = mobRespawn.position;
        mobRoot.localRotation = Quaternion.identity;
        onMobMovedToRespawnPosition?.Invoke();

        mob.combatantAnimator.SetTrigger("respawn");

        mobRoot
            .DOLocalMove(Vector3.zero, returnDuration)
            .OnKill( onMobReset.Invoke );
    }
}
