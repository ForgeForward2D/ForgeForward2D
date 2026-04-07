using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializableEntry<K, V>> entries = new List<SerializableEntry<K, V>>();

    public void OnBeforeSerialize()
    {
        entries.Clear();
        using Enumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<K, V> current = enumerator.Current;
            entries.Add(new SerializableEntry<K, V>(current.Key, current.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();
        for (int i = 0; i < entries.Count; i++)
        {
            Add(entries[i].Key, entries[i].Value);
        }

        entries.Clear();
    }
}

[System.Serializable]
public struct SerializableEntry<K, V>
{
    public K Key;
    public V Value;

    public SerializableEntry(K key, V value)
    {
        Key = key;
        Value = value;
    }
}