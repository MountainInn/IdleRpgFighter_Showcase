using Zenject;
using UnityEngine;

public class CharacterView : MobView
{
    [Inject]
    public void Subscribe(Character character)
    {
        healthBar.Subscribe(character.gameObject, character.health);
        energyBar.Subscribe(character.gameObject, character.energy);
    }
}
