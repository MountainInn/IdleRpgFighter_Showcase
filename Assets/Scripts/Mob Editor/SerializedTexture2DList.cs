using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public class SerializedTexture2DList : IList<Texture2D>, ISerializationCallbackReceiver
{
    [SerializeField] List<byte[]> pngs;

    List<Texture2D> textures;


    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        textures =
            pngs
            .Select(png =>
            {
                Texture2D texture = new Texture2D(50, 50);
                ImageConversion.LoadImage(texture, png);
                return texture;
            })
            .ToList();
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        pngs =
            textures
            .Select( ImageConversion.EncodeToPNG )
            .ToList();
    }


    public Texture2D this[int index] { get => ((IList<Texture2D>)textures)[index]; set => ((IList<Texture2D>)textures)[index] = value; }

    public int Count => ((ICollection<Texture2D>)textures).Count;

    public bool IsReadOnly => ((ICollection<Texture2D>)textures).IsReadOnly;

    public void Add(Texture2D item)
    {
        ((ICollection<Texture2D>)textures).Add(item);
    }

    public void Clear()
    {
        ((ICollection<Texture2D>)textures).Clear();
    }

    public bool Contains(Texture2D item)
    {
        return ((ICollection<Texture2D>)textures).Contains(item);
    }

    public void CopyTo(Texture2D[] array, int arrayIndex)
    {
        ((ICollection<Texture2D>)textures).CopyTo(array, arrayIndex);
    }

    public IEnumerator<Texture2D> GetEnumerator()
    {
        return ((IEnumerable<Texture2D>)textures).GetEnumerator();
    }

    public int IndexOf(Texture2D item)
    {
        return ((IList<Texture2D>)textures).IndexOf(item);
    }

    public void Insert(int index, Texture2D item)
    {
        ((IList<Texture2D>)textures).Insert(index, item);
    }

    public bool Remove(Texture2D item)
    {
        return ((ICollection<Texture2D>)textures).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<Texture2D>)textures).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)textures).GetEnumerator();
    }
}
