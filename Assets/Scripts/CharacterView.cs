using Zenject;
using UnityEngine;

public class CharacterView : MobView
{
    [SerializeField] ProgressBar energyBar;

    [Inject]
    public void Subscribe(Character character)
    {
        healthBar.Subscribe(character.gameObject, character.health);
        energyBar.Subscribe(character.gameObject, character.energy);
    }
}
