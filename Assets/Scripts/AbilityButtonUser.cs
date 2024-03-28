using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AbilityButtonUser : MonoBehaviour
{
    [Inject]
    public void Construct(DiContainer Container,
                          Character character,
                          TalentUser talentUser)
    {
        talentUser.abilities
            .Map(a =>
            {
                var abilityButton = Container.Resolve<AbilityButton>();

                Container.Inject(a);

                a.SubscribeButton(character, abilityButton);
            });
    }
}
