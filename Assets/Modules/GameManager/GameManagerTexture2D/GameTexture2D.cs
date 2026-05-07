using System;
using UnityEngine;

[Serializable]
public class GameTexture2D
{
    [Tooltip("Nombre de nuestra textura 2D.")]
    public string Name;

    [Tooltip("Id de nuestra textura 2D.")]
    public int GameTexture2DId = 0;

    [Tooltip("Llave secundaria para identificar la textura, trabaja en conjunto con 'Key'.")]
    public GameTexture2DUsage Usage = GameTexture2DUsage.Any;

    [Tooltip("Llave terciaria para identificar la textura.")]
    public int Key = 0;

    [Tooltip("Icono.")]
    public Texture2D Texture;

    [Tooltip("Color.")]
    public Color32 Color;
}