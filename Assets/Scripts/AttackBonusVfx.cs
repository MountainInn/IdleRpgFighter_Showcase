using UnityEngine;
using DG.Tweening;
using Zenject;

public class AttackBonusVfx : MonoBehaviour
{
    [SerializeField] float fadeDuration;
    [SerializeField] MeshRenderer meshRenderer;

    [Inject]
    public void Construct(Character character)
    {
        transform.position = character.transform.position;
    }

    public void ShowBonus()
    {
        meshRenderer
            .material
            .DOFade(1f, fadeDuration);
    }

    public void HideBonus()
    {
        meshRenderer
            .material
            .DOFade(0f, fadeDuration);
    }
}
