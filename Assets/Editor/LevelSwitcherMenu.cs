using UnityEditor;
using UnityEngine;

public static class LevelSwitcherMenu
{
    [MenuItem("🔂 Level Switcher/Arena")]
    static public void LoadArena()
    {
        GameObject
            .FindObjectOfType<LevelSwitcher>()
            ?.SwitchToArena();
    }

    [MenuItem("🔂 Level Switcher/Gulag")]
    static public void LoadGulag()
    {
        GameObject
            .FindObjectOfType<LevelSwitcher>()
            ?.SwitchToGulag();
    }
}
