using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenuAttribute(fileName = "TemplateEditorData", menuName = "S Singleton/TemplateEditorData")]
public class TemplateEditorData : ScriptableSingleton<TemplateEditorData>
{
    [SerializeField] public GameObject modularCharacterPrefab;

    [SerializeField] public SerializedDictionary<string, ListObject<SkinnedMeshRenderer>> parts;
    [SerializeField] public SerializedDictionary<SkinnedMeshRenderer, SerializedTexture2D> previews;


    [MenuItem("Template Editor/Rebuild")]
    static public void RebuildButton()
    {
        TemplateEditorData.instance.Rebuild();
    }

    void Awake()
    {
        Rebuild();
    }

    void Rebuild()
    {
        modularCharacterPrefab =
            PrefabUtility .LoadPrefabContents("Assets/Resources/UnityStoreAssets/PolygonFantasyHeroCharacters/Prefabs/Characters_Presets/Chr_FantasyHero_Preset_1.prefab");


        parts = new();
        previews = new();

        modularCharacterPrefab
            .GetComponentsInChildren<SkinnedMeshRenderer>(true)
            .Map(part =>
            {
                if (part?.sharedMesh == null)
                    return;

                var parent = part.transform.parent.gameObject;

                parts.TryAdd(parent.name, new());

                parts[parent.name].Add(part);

                MeshPreview meshPreview = new MeshPreview(part.sharedMesh);

                Texture2D preview = meshPreview.RenderStaticPreview(70, 70);

                previews.Add(part, preview);

                meshPreview.Dispose();
            });

        Debug.Log($"Rebuilt");
    }
}
