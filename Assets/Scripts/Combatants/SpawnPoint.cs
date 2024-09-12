using UnityEngine;

abstract public class SpawnPoint : MonoBehaviour
{
    public void ApplyPosition(Transform combatantTransform)
    {
        transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

        combatantTransform.SetPositionAndRotation(position, rotation);
    }
}
