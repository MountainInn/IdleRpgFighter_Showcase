using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Afterimage : MonoBehaviour
{
    [SerializeField] bool enable;
    [Space]
    [SerializeField] int afterimageCount;
    [SerializeField] int afterimageLifetimeDivisor = 4;
    [Space]
    [SerializeField] Animator afterimagePrefab;
    [SerializeField] TrailRenderer trail;

    List<Animator> afterimages;

    [Inject] Character character;
    [Inject] CharacterSpawnPoint spawnPoint;
    [Inject]
    public void Construct()
    {
        if (!enable)
            return;
        // trail.emitting = false;

        afterimages =
            afterimageCount
            .ToRange()
            .Select(_ =>
            {
                var afterimage =
                    Instantiate(afterimagePrefab, character.transform.parent)
                    .GetComponent<Animator>();

                afterimage.speed = 0;

                afterimage.transform.localScale = character.transform.localScale;

                spawnPoint.transform.GetPositionAndRotation(out var pos, out var rot);

                afterimage.transform.SetPositionAndRotation(pos, rot);

                return afterimage;
            })
            .ToList();

        if (character.ObserveStateMachine == null)
            character.InitObserveStateMachine();

        character.ObserveStateMachine
            .OnStateEnterAsObservable()
            .Subscribe(_ =>
            {
                if (!_.StateInfo.IsTag("attack"))
                {
                    trail.emitting = false;
                    return;
                }

                AnimatorClipInfo[] animatorClipInfos =
                    character
                    .combatantAnimator
                    .GetNextAnimatorClipInfo(0);

                if (animatorClipInfos.None())
                    return;

                string name = animatorClipInfos.ElementAt(0).clip.name;

                _.Animator.Play(name, 0, .8f);

                AnimatorStateInfo animatorStateInfo =
                    character
                    .combatantAnimator
                    .GetNextAnimatorStateInfo(0);

                MakeAfterimage(animatorStateInfo);

                // Vector3 swordTrailPosition =
                //     _
                //     .Animator
                //     .GetComponent<SwordEdgePosition>()
                //     .swordTransform
                //     .position;

                // trail.AddPosition(swordTrailPosition);

                // trail.emitting = true;
            })
            .AddTo(this);
    }

    void MakeAfterimage(AnimatorStateInfo state)
    {
        foreach (var (i, afterimage) in afterimages.Enumerate())
        {
            afterimage.gameObject.SetActive(true);
           
            float time = i / (float)afterimages.Count;

            afterimage.Play(state.fullPathHash, 0, time);

            this.StartInvokeAfter(() => afterimage.gameObject.SetActive(false),
                                  time/ afterimageLifetimeDivisor);

            // Vector3 swordTrailPosition =
            //     afterimage
            //     .GetComponent<SwordEdgePosition>()
            //     .swordTransform
            //     .position;

            // trail.AddPosition(swordTrailPosition);
        };
    }
}
