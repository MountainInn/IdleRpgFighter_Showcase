public class AttackBuff : Buff
{
    public override void Subscribe(Combatant combatant)
    {
        combatant.preAttack.AddListener(args =>
        {
            args.damage *= activeBonus;
        });
    }
}
