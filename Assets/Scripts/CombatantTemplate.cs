using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CombatantTemplate", menuName = "SO/CombatantTemplate")]
public class CombatantTemplate : ScriptableObject
{
    [SerializeField] public GameObject modularCharacterPrefab;

    [System.Serializable]
    public struct Entry
    {
        [SerializeField] public string name;
        [SerializeField] public bool toggle;

        public void Deconstruct(out string name, out bool toggle)
        {
            name = this.name;
            toggle = this.toggle;
        }

        public override string ToString()
        {
            return $"({name} : {toggle})\n";
        }
    }

    [SerializeField] public List<Entry> _toggles;

    public Dictionary<string, bool> toggles
    {
        set {
            _toggles =
                value
                .Select(kv => new Entry(){ name = kv.Key, toggle = kv.Value })
                .ToList();
        }

        get {
            cachedDictToggles ??=
                _toggles
                .ToDictionary(entry => entry.name,
                              entry => entry.toggle);

            return cachedDictToggles;
        }
    }

    Dictionary<string, bool> cachedDictToggles;

    Dictionary<string, GameObject> prefabChildren;

    GameObject cachedModularCombatant;

    public void ApplyTemplate(GameObject modularCombatant)
    {
        if (cachedModularCombatant != modularCombatant)
        {
            cachedModularCombatant = modularCombatant;
           
            prefabChildren =
                modularCombatant
                .GetComponentsInChildren<Transform>(true)
                .Where(child => cachedDictToggles.Keys.Contains(child.name))
                .ToDictionary(child => child.name,
                              child => child.gameObject);
        }

        foreach (var (name, toggle) in toggles)
        {
            prefabChildren[name]
                .gameObject
                .SetActive(toggle);
        }
    }

    public void Save()
    {
        toggles = cachedDictToggles;
    }

    public void ClearCachedDictToggles()
    {
        cachedDictToggles?.Clear();
        cachedDictToggles = null;
    }
}
