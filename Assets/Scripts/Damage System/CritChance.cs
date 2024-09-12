
public class CritChance : DamageModifier
{
    public override void Initialize()
    {
        character.preAttack.AddListener(CritRoll);
    }

    private void CritRoll(DamageArgs args)
    {
        if (UnityEngine.Random.value <= character.Stats.critChance)
        {
            args.damage *= character.Stats.critMult;
            args.isCrit = true;
        }
    }
}
