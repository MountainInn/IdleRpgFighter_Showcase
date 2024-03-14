using UnityEngine;
using DG.Tweening;
using Zenject;

public class BlockVfx : MonoBehaviour
{
    [SerializeField] float blockFadeDuration = .1f;
    [Space]
    [SerializeField] MeshRenderer blockRenderer;

    [SerializeField] Color blockColor = Color.blue;
    [SerializeField] Color parryColor = Color.red;

    [Inject]
    public void Construct(Character character)
    {
        transform.position = character.transform.position;
    }

    void Awake()
    {
        HideBlock();
        HideParry();
    }

    public void ShowBlock()
    {
        blockRenderer
            .material
            .DOFade(1f, blockFadeDuration);
    }

    public void HideBlock()
    {
        blockRenderer
            .material
            .DOFade(0f, blockFadeDuration);
    }

    public void ShowParry()
    {
        blockRenderer
            .material
            .DOColor(parryColor, blockFadeDuration);
    }

    public void HideParry()
    {
        blockRenderer
            .material
            .DOColor(blockColor, blockFadeDuration);
    }

    public void ShowCounterAttack()
    {
        blockRenderer
            .material
            .DOColor(Color.yellow, blockFadeDuration)
            .SetEase(Ease.Flash, 2);
    }
}
