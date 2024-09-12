using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Afterimage : MonoBehaviour
{
    [SerializeField] int afterimageCount;
    [SerializeField] int afterimageLifetimeDivisor = 4;
    [Space]
    [SerializeField] Animator afterimagePrefab;
    [SerializeField] TrailRenderer trail;

    List<Animator> afterimages;

    [Inject] Character character;
    [Inject] CharacterSpawnPoint spawnPoint;
    [Inject] LightSpeedMode lightSpeedMode;
    [Inject]
    public void Construct()
    {
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

                afterimage.gameObject.SetActive(false);

                return afterimage;
            })
            .ToList();

        lightSpeedMode.enabled
            .WhereEqual(true)
            .Subscribe(_ =>
            {
                character.ObserveStateMachine
                    .OnStateEnterAsObservable()
                    .TakeUntil( lightSpeedMode.enabled.WhereEqual(false) )
                    .Subscribe(_ =>
                    {
                        if (!_.StateInfo.IsTag(lightSpeedMode.noTimeAttack_AnimationTag))
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

                        AnimationClip clip = animatorClipInfos.ElementAt(0).clip;

                        string name = clip.name;
                        float time = 0.8f;

                        _.Animator.Play(name, 0, time);

                        MakeAfterimage(name, time);

                    })
                    .AddTo(this);
            })
            .AddTo(this);
    }

    void MakeAfterimage(string name, float endTime)
    {
        foreach (var (i, afterimage) in afterimages.Enumerate())
        {
            afterimage.gameObject.SetActive(true);
           
            float time = i / (float)afterimages.Count * endTime;

            afterimage.Play(name, 0, time);

            this.StartInvokeAfter(() => afterimage.gameObject.SetActive(false),
                                  time/ afterimageLifetimeDivisor);
        };
    }
}
