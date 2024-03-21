using UnityEditor;

public static class GameSettingsMenu
{
    [MenuItem("☸ Game Settings/Open")]
    static public void Open()
    {
        Selection.objects = new[] {
            AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Resources/GameSettings.asset")
        };
    }
}
