using System;

[Flags]
public enum UnitType
{
    /// <summary>
    /// Indica que no es algun tipo de unidad.
    /// </summary>
    None = 0,
    /// <summary>
    /// Indica que es una unidad.
    /// </summary>
    Unit = 1,
    /// <summary>
    /// Indica que es una trampa.
    /// </summary>
    Trap = 2,
    /// <summary>
    /// Indica que es de efecto.
    /// </summary>
    Effect = 4,
    /// <summary>
    /// Indica que es un trofeo.
    /// </summary>
    Trophy = 8,
    /// <summary>
    /// Indica que es un tesoro.
    /// </summary>
    Treasure = 16,
}