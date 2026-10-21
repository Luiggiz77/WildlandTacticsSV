using System;

[Serializable]
public class GameplayEffect
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id;

    /// <summary>
    /// Tipo de efecto.
    /// </summary>
    public GameplayEffectType Type;

    /// <summary>
    /// Objetivo de efecto.
    /// </summary>
    public GameplayTarget Target;

    /// <summary>
    /// Tipo de area de efecto.
    /// </summary>
    public GameplayEffectAreaType AreaType;

    /// <summary>
    /// Valor que indica el area afectada.
    /// </summary>
    public int Area;

    /// <summary>
    /// Valor que puede representar tanto un id de un objeto como significar un valor para la propiedad que afecta.
    /// </summary>
    public int Value;

    /// <summary>
    /// Indica cuantas veces se puede ejecutar el efecto. Nota: 1 indica que es solo una vez y cero ninguna mas.
    /// </summary>
    public int Executions;

    public int UnitPropertiesId;
}