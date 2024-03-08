using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CombatantTemplate", menuName = "SO/CombatantTemplate")]
public class CombatantTemplate : ScriptableObject
{
    [SerializeField] public Dictionary<string, bool> toggles
    {
        get
        {
            _toggles ??=
                Enumerable
                .Zip(names, bools, (name, b) => (name, b))
                .ToDict();

            return _toggles;
        }
        set
        {
            names = value.Keys.ToList();
            bools = value.Values.ToList();
        }
    }

    Dictionary<string, bool> _toggles;

    [SerializeField] public List<string> names;
    [SerializeField] public List<bool> bools;

    public bool this[string name]
    {
        get
        {
            return toggles[name];
        }
        set
        {
            toggles[name] = value;
            int index = names.IndexOf(name);
            bools[index] = value;
        }
    }


    Dictionary<string, GameObject> prefabChildren;

    public void ApplyTemplate(GameObject modularCombatant)
    {
        InitPrefabChildren(modularCombatant);

        foreach (var (name, toggle) in toggles)
        {
            var part = prefabChildren[name].gameObject;


            if (toggle)
            {
                for (GameObject parent = part;
                     parent != modularCombatant;
                     parent = parent.transform.parent.gameObject)
                {
                    parent.SetActive(true);
                }
            }
            else
            {
                part.SetActive(false);
            }
        }
    }

    private void InitPrefabChildren(GameObject modularCombatant)
    {
        prefabChildren =
            modularCombatant
            .GetComponentsInChildren<Transform>(true)
            .Where(child => toggles.Keys.Contains(child.name))
            .ToDictionary(child => child.name,
                          child => child.gameObject);
    }
}
