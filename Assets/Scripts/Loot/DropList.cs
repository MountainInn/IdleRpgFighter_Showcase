using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "DropList", menuName = "SO/DropList")]
public class DropList : ScriptableObject
{
    [SerializeField] public List<Entry> entries = new();

    [Serializable]
    public class Entry
    {
        [Range(0, 1)] public float chance;
        public Drop drop;
    }
}
