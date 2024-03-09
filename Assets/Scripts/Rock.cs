using Zenject;

public class Rock : Combatant
{
    [Inject]
    public void Construct()
    {
        base.SetStats(Stats);
    }
}
