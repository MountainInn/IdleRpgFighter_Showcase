using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DropList", menuName = "SO/DropList")]
public class DropList : ScriptableObject
{
    [SerializeField] public List<Entry> entries;

    [Serializable]
    public class Entry
    {
        [Range(0, 1)] public float chance;
        public Price currency;
        public ScriptableObject sdf;
    }
}
