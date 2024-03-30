public class AttackBuff : Buff
{
    public override void Subscribe(AnimatorCombatant combatant)
    {
        combatant.preAttack.AddListener(args =>
        {
            args.damage *= activeBonus;
        });
    }
}
