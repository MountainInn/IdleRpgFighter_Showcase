using Zenject;

public class Rock : Combatant
{
    [Inject]
    public void Construct()
    {
        Stats.Apply(this);
    }
}
