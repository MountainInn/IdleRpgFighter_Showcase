using UnityEngine;
using Zenject;

public class CharacterSpawnPoint : SpawnPoint
{
    [Inject]
    public void Construct(Character character)
    {
        ApplyPosition(character.transform);
    }
}
