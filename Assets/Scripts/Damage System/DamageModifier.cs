using Zenject;

public abstract class DamageModifier : IInitializable
{
    [Inject] protected Character character;

    public abstract void Initialize();
}
