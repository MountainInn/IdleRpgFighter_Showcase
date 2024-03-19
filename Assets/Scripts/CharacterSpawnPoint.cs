using UnityEngine;
using Zenject;

public class CharacterSpawnPoint : MonoBehaviour
{
    [Inject]
    public void Construct(Character character)
    {
        transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

        character.transform.SetPositionAndRotation(position, rotation);
    }
}
