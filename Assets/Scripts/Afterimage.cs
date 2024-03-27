using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Afterimage : MonoBehaviour
{
    [SerializeField] int afterimageCount;
    [Space]
    [SerializeField] Animator afterimagePrefab;

    List<Animator> afterimages;

    [Inject] Character character;
    [Inject] CharacterSpawnPoint spawnPoint;
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
                    return;

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
            })
            .AddTo(this);
    }

    void MakeAfterimage(AnimatorStateInfo state)
    {
        foreach (var (i, afterimage) in afterimages.Enumerate())
        {
            float time = i / (float)afterimages.Count;

            afterimage.Play(state.fullPathHash, 0, time);
        };
    }
}
