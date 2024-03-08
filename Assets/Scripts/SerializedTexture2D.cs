using System;
using UnityEngine;

[Serializable]
public class SerializedTexture2D : ISerializationCallbackReceiver
{
    [SerializeField] byte[] png;

    public Texture2D texture;

    public SerializedTexture2D(Texture2D texture)
    {
        this.texture = texture;
    }

    static public implicit operator Texture2D(SerializedTexture2D serializedTexture)
    {
        if (serializedTexture.texture == null)
        {
            serializedTexture.texture = new Texture2D(50, 50);
            ImageConversion.LoadImage(serializedTexture.texture, serializedTexture.png);
        }

        return serializedTexture.texture;
    }
    static public implicit operator SerializedTexture2D(Texture2D texture)
    {
        return new SerializedTexture2D(texture);
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (png == null)
            png = ImageConversion.EncodeToPNG(texture);
    }
}
