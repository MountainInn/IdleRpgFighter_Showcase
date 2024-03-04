using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CombatantTemplate))]
public class CombatantTemplateEditor : IsolationEditor
{
    CombatantTemplate template;

    GameObject combatantPreview;

    Dictionary<GameObject, HashSet<SkinnedMeshRenderer>> dictParts;
    Dictionary<SkinnedMeshRenderer, Texture2D> dictPreviewTextures;
    Dictionary<GameObject, bool> foldout;

    GUIStyle baseStyle, selectedStyle;

    void OnEnable()
    {
        isEditing = false;
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

    }

    protected override void OnStartEditing()
    {
        if (dictParts == null)
        {
            InitializeParts();
        }
    }

    protected override IEnumerable<GameObject> InitPreviews()
    {
        combatantPreview = GameObject.Instantiate(template.modularCharacterPrefab);
        combatantPreview.name = "[Combatant Template Preview]";

        return new []
        {
            combatantPreview
        };
    }

    protected override Vector3 GetSceneViewLookAtPosition()
    {
        return Vector3.zero;
    }

    protected override void ConcreteOnSceneGUI()
    {
        Handles.BeginGUI();

        if (GUI.Button(new Rect(10, 200, 100, 100), "Finish Editing"))
        {
            StopEditing();
        }

        Handles.EndGUI();
    }

    protected override void UpdatePreviewsOnSceneGUI()
    {
    }

    protected override void ConcreteOnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        template.modularCharacterPrefab =
            (GameObject)
            EditorGUILayout
            .ObjectField(template.modularCharacterPrefab, typeof(GameObject), false);

        if (EditorGUI.EndChangeCheck())
        {
            SkinnedMeshRenderer[] parts = InitializeParts();

            template.ClearCachedDictToggles();
           
            template.toggles =
                parts
                .ToDictionary(part => part.name,
                              _ => false);
        }

        if (template?.modularCharacterPrefab == null)
        {
            EditorGUILayout.LabelField("PREFAB IS NOT SET");
            return;
        }

        if (isEditing)
        {
            foreach (var (parent, parts) in dictParts)
            {
                bool hasToggledParts = parts.Any(part => template.toggles[part.name]);

                GUIStyle style = (hasToggledParts) ? selectedStyle : baseStyle;

                foldout[parent] =
                    EditorGUILayout.BeginFoldoutHeaderGroup(foldout[parent],
                                                            parent.name,
                                                            style);

                if (foldout[parent])
                {
                    parts
                        .Chunks(3)
                        .Map(row =>
                        {
                            EditorGUILayout.BeginHorizontal();

                            foreach (var part in row)
                            {
                                string name = part.name;

                                bool toggle = template.toggles[name];

                                style = (toggle) ? selectedStyle : baseStyle;

                                bool clicked = GUILayout.Button(dictPreviewTextures[part],
                                                                style);

                                if (clicked)
                                    template.toggles[name] = !toggle;
                            }

                            EditorGUILayout.EndHorizontal();
                        });
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }

    private SkinnedMeshRenderer[] InitializeParts()
    {
        dictParts = new();
        dictPreviewTextures = new();
        foldout = new();

        var parts =
            template.modularCharacterPrefab
            .GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (var part in parts)
        {
            if (part?.sharedMesh == null)
                continue;


            GameObject parent = part.transform.parent.gameObject;

            dictParts.TryAdd(parent, new());
            dictParts[parent].Add(part);

            foldout.TryAdd(parent, true);

            MeshPreview meshPreview = new(part.sharedMesh);

            Texture2D texture = meshPreview.RenderStaticPreview(70, 70);

            meshPreview.Dispose();

            dictPreviewTextures.Add(part, texture);
        };

        return parts;
    }

    protected override void UpdatePreviewsOnInspectorGUI()
    {
        if (isEditing)
            template.ApplyTemplate(combatantPreview);
    }

    protected override void FinalizeTargetObject()
    {
        template.Save();
    }
}
