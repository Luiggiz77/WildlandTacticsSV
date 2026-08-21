using System;

[Serializable]
public class UnitProperties
{
    /// <summary>
    /// Llave de las propiedades de la unidad.
    /// </summary>
    public int Id;

    /// <summary>
    /// Id de las propiedades de batalla.
    /// </summary>
    public int UnitBattlePropertiesId;

    /// <summary>
    /// Llave de la plantilla de propiedades de la unidad en batalla usados.
    /// </summary>
    public UnitBattleProperties UnitBattleProperties;

    /// <summary>
    /// Nombre de la unidad.
    /// </summary>
    public string Name;

    /// <summary>
    /// Elementos visuales de la unidad.
    /// </summary>
    public UnitVisualElement[] UnitVisualElements;
}