using System;

[Serializable]
public class UnitProperties
{
    /// <summary>
    /// Llave de la configuración de la unidad.
    /// </summary>
    public int Id;

    /// <summary>
    /// Llave de la plantilla de propiedades de la unidad en batalla usados.
    /// </summary>
    public int UnitBattlePropertiesId;

    /// <summary>
    /// Nombre de la unidad.
    /// </summary>
    public string Name;
}