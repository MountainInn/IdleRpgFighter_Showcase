using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenuAttribute(fileName = "TemplateEditorData", menuName = "S Singleton/TemplateEditorData")]
public class TemplateEditorData : ScriptableSingleton<TemplateEditorData>
{
    [SerializeField] public GameObject modularCharacterPrefab;

    [SerializeField] public SerializedDictionary<string, ListObject<SkinnedMeshRenderer>> parts;
    [SerializeField] public SerializedDictionary<SkinnedMeshRenderer, SerializedTexture2D> previews;

    public void Save()
    {
        Save(true);
    }
}
