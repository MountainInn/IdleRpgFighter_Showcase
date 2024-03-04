using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenuAttribute(fileName = "TemplateEditorData", menuName = "S Singleton/TemplateEditorData")]
public class TemplateEditorData : ScriptableSingleton<TemplateEditorData>, ISerializationCallbackReceiver
{
    [SerializeField] public GameObject modularCharacterPrefab;

    [HideInInspector] [SerializeField] public List<GameObject> parents;
    [HideInInspector] [SerializeField] public List<GameObject> parts;
    [SerializeField] public List<byte[]> previews;

    public Dictionary<GameObject, HashSet<GameObject>> dictParts;
    public Dictionary<GameObject, Texture2D> dictPreviews;

    public void Rebuild()
    {
        if (modularCharacterPrefab == null)
        {
            Debug.LogWarning($"modularCharacterPrefab is null!");
            return;
        }

        parents = new();
        parts = new();
        previews = new();
           
        modularCharacterPrefab
            .GetComponentsInChildren<SkinnedMeshRenderer>()
            .Map(part =>
            {
                var parent = part.transform.parent.gameObject;


                parents.Add(parent);
                parts.Add(part.gameObject);
            });

        Save();

        InitDictionaries();

        Debug.Log($"REbuildt");
    }


    public void Save()
    {
        Save(true);
        // EditorUtility.RequestScriptReload();
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        InitDictionaries();
    }

    public void InitDictionaries()
    {
        dictParts = new();

        parents.Count.ForLoop(i =>
        {
            dictParts.TryAdd(parents[i], new());

            dictParts[parents[i]].Add(parts[i]);
        });
    }
}
