using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Price))]
public class PriceDrawer : PropertyDrawer
{
    GUILayoutOption
        width = GUILayout.Width(10),
        width2 = GUILayout.Width(50),
        width3 = GUILayout.Width(100)
        ;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(property.name);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("currency"),
                                          new GUIContent(""),
                                          width3);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("cost"),
                                          new GUIContent(""),
                                          width3);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndProperty();
    }
}
