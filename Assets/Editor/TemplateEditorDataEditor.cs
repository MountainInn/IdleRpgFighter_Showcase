using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TemplateEditorData))]
public class TemplateEditorDataEditor : Editor
{
    Rect previewRect = new Rect(0, 0, 70, 70);

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var t = (target as TemplateEditorData);

        if (t.modularCharacterPrefab == null)
        {
            EditorGUILayout.LabelField("PREFAB IS NOT SET");
            return;
        }

        if ( GUILayout.Button("Rebuild") )
        {
            t.parts = new();
            t.previews = new();

            t.modularCharacterPrefab
                .GetComponentsInChildren<SkinnedMeshRenderer>(true)
                .Map(part =>
                {
                    if (part?.sharedMesh == null)
                        return;

                    var parent = part.transform.parent.gameObject;

                    t.parts.TryAdd(parent.name, new());

                    t.parts[parent.name].Add(part);

                    MeshPreview meshPreview = new MeshPreview(part.sharedMesh);

                    Texture2D preview =
                        meshPreview.RenderStaticPreview((int)previewRect.width,
                                                        (int)previewRect.height);

                    t.previews.Add(part, preview);

                    meshPreview.Dispose();
                });

            t.Save();
                
            Debug.Log($"Rebuilt");
        }
    }
}
