using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Gang : MonoBehaviour
{
    [SerializeField] int maxAllies;
    [SerializeField] float angleIncrement = 20;

    List<Ally> allies = new();

    float angle;

    Vector3 mobPosition;
    Vector3 fromMobToChar = Vector3.left;

    public void Initialize(Mob mob, Character character)
    {
        mobPosition = mob.transform.position;
        fromMobToChar = (character.transform.position - mobPosition);
    }

    public void Add(Ally ally)
    {
        if (allies.Count % 2 == 0)
            angle += angleIncrement * Mathf.Sign(angle);
        else
            angle *= -1;

        Vector3 allyPosition =
            mobPosition +
            Quaternion.Euler(0, angle, 0) * fromMobToChar;

        Debug.Log($"{angle}");

        ally.transform.position = allyPosition;
        ally.transform.rotation = Quaternion.LookRotation(mobPosition - allyPosition);

        allies.Add(ally);
    }
}
