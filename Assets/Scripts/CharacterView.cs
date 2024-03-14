using Zenject;

public class CharacterView : MobView
{
    [Inject]
    public void Subscribe(Character character)
    {
        healthBar.Subscribe(character.gameObject, character.health);
    }
}
