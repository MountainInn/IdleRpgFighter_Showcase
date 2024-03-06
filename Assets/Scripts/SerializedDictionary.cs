using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<TKey> keys;
    [SerializeField] List<TValue> values;

    Dictionary<TKey, TValue> dict;

    public SerializedDictionary()
    {
        dict = new();
        keys = new();
        values = new();
    }

    static public implicit operator SerializedDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        return new SerializedDictionary<TKey, TValue>(){ dict = dictionary };
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (dict.Keys.Any() && keys.None())
        {
            keys = dict.Keys.ToList();
            values = dict.Values.ToList();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (dict.None() && keys.Any())
        {
            dict =
                Enumerable
                .Zip(keys, values,
                     (key, val) => new KeyValuePair<TKey, TValue>(key, val))
                .ToDictionary();
        }
    }


    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)dict)[key];
        set => ((IDictionary<TKey, TValue>)dict)[key] = value;
    }

    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)dict).Keys;

    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)dict).Values;

    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)dict).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dict).IsReadOnly;

    public void Add(TKey key, TValue value)
    {
        ((IDictionary<TKey, TValue>)dict).Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dict).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dict).Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)dict).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)dict).GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        return ((IDictionary<TKey, TValue>)dict).Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return ((IDictionary<TKey, TValue>)dict).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)dict).GetEnumerator();
    }
}
