using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TemplateEditorData))]
public class TemplateEditorDataEditor : Editor
{
    List<byte[]> previews;

    Rect previewRect = new Rect(0, 0, 70, 70);

    List<Texture2D> Textures;

    void OnEnable()
    {
        previews ??= new();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var t = (target as TemplateEditorData);

        if ( GUILayout.Button("Rebuild") )
        {
            (target as TemplateEditorData)
                .Rebuild();

            Debug.Log($"Rebuilt");
        }

        if ( GUILayout.Button("Generate Previews") )
        {
            previews = new();

            Textures = new();

            foreach (var part in t.parts)
            {
                SkinnedMeshRenderer meshRenderer = part.GetComponent<SkinnedMeshRenderer>();

                using (MeshPreview meshPreview = new MeshPreview(meshRenderer.sharedMesh))
                {
                    Texture2D preview = meshPreview.RenderStaticPreview((int)previewRect.width,
                                                                        (int)previewRect.height);

                    Textures.Add(preview);

                    byte[] bytes = ImageConversion.EncodeToPNG(preview);

                    meshPreview.Dispose();

                    previews.Add(bytes);
                }
            }

            t.previews = previews;

            t.Save();

            t.InitDictionaries();

            t.dictPreviews = new();

            t.parents.Count.ForLoop(i =>
            {
// Texture2D loadedTex = new Texture2D(70, 70);

// ImageConversion.LoadImage(loadedTex, t.previews[i]);

// t.dictPreviews?.TryAdd(t.parts[i], loadedTex);
                });

            Debug.Log($"Rendered");
        }


        Textures
            .Map(texture =>
            {
                GUILayout.Button(texture);
            });

    }

}
