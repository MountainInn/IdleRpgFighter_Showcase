using UnityEditor;

[CustomEditor(typeof(Mob))]
public class MobEditor : Editor
{
    Mob mob;

    void OnEnable()
    {
        mob ??= (target as Mob);
    }

    public override void OnInspectorGUI()
    {
        CombatantTemplate before = mob.startingMobStats.template;

        base.OnInspectorGUI();

        CombatantTemplate after = mob.startingMobStats.template;

        if (before != after)
        {
            after.ApplyTemplate(mob.gameObject);
        }
    }

}
