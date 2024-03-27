using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TalentUser : MonoBehaviour
{
    [Inject] public List<Talent> talents;
    [Inject] public List<Ability> abilities;

    public bool alreadyInjected;
}
