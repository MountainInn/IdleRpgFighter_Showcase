using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TalentUser : MonoBehaviour
{
    [Inject] List<Talent> talents;
    [Inject] List<Ability> abilities;

    public bool alreadyInjected;
}
