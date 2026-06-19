using System;
using UnityEngine;

[Serializable]
public class GameTexture2D
{
    [Tooltip("Llave terciaria para identificar la textura.")]
    public int Key = 0;

    [Tooltip("Color.")]
    public Color32 Color;

    [Tooltip("Icono.")]
    public Texture2D Texture;

    public GameTexture2D() { }
    public GameTexture2D(int lnKey, Color loColor, Texture2D loTexture2D)
    {
        Key = lnKey;
        Color = loColor;
        Texture = loTexture2D;
    }
}