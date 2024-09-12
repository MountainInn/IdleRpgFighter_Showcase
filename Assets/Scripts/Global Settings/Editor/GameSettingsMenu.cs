using UnityEditor;

public static class GameSettingsMenu
{
    private const string cheatsPath = "Assets/Resources/Cheats.asset";

    [MenuItem("☸ Configs/Global Game Settings")]
    static public void OpenGlobalGameSettings()
    {
        Selection.objects = new[] {
            AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Resources/GameSettings.asset")
        };
    }

    [MenuItem("☸ Configs/Cheats")]
    static public void OpenCheats()
    {
        Cheats cheats = AssetDatabase.LoadAssetAtPath<Cheats>(cheatsPath);

        if (cheats == null)
        {
            AssetDatabase.CreateAsset(new Cheats(), cheatsPath);
        }

        cheats = AssetDatabase.LoadAssetAtPath<Cheats>(cheatsPath);

        Selection.objects = new[] {
            cheats
        };
    }

}
