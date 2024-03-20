using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CombatantTemplate))]
public class CombatantTemplateEditor : Editor
{
    CombatantTemplate template;

    Dictionary<string, bool> foldout;

    GUIStyle baseStyle, selectedStyle;

    void OnEnable()
    {
        template ??= (CombatantTemplate)target;

        baseStyle =
            new GUIStyle()
            {
                normal = new GUIStyleState(){ textColor = Color.black,
                                              background = Texture2D.grayTexture},

                border = new RectOffset(1, 1, 1, 1),
                margin = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(12, 2, 5, 5),
                alignment = TextAnchor.MiddleCenter,
            };

        selectedStyle = new GUIStyle(baseStyle)
        {
            normal = new GUIStyleState(){ textColor = Color.black,
                                          background = Texture2D.whiteTexture},
        };

        InitializeFoldouts();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reinitialize"))
        {
            template.toggles =
                TemplateEditorData.instance.parts
                .SelectMany(kv =>
                {
                    return kv.Value.list;
                })
                .ToDictionary(part => part.name,
                              part => false);
        }
       
        foreach (var (parent, parts) in TemplateEditorData.instance.parts)
        {
            bool hasToggledParts = parts.Any(part => template.toggles[part.name]);

            GUIStyle style = (hasToggledParts) ? selectedStyle : baseStyle;

            foldout[parent] =
                EditorGUILayout.BeginFoldoutHeaderGroup(foldout[parent],
                                                        parent,
                                                        style);
            if (foldout[parent])
            {
                parts
                    .Chunks(6)
                    .Map(row =>
                    {
                        EditorGUILayout.BeginHorizontal();

                        foreach (var part in row)
                        {
                            string name = part.name;

                            bool toggle = template.toggles[name];

                            style = (toggle) ? selectedStyle : baseStyle;

                            Texture2D texture = TemplateEditorData.instance.previews[part];

                            bool clicked = GUILayout.Button(texture,
                                                            style,
                                                            GUILayout.MinWidth(10));

                            if (clicked)
                            {
                                template[name] = !toggle;

                                EditorUtility.SetDirty(template);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    });
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

    }


    private void InitializeFoldouts()
    {
        foldout = new();

        foreach (var (parent, parts) in TemplateEditorData.instance.parts)
        {
            foldout.TryAdd(parent, true);
        };
    }
}
