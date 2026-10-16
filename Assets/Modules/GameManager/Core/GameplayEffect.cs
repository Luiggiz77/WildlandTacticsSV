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
    public GameplayEffectType Type { get; set; }

    /// <summary>
    /// Valor que puede representar tanto un id de un objeto como significar un valor para la propiedad que afecta.
    /// </summary>
    public int Value { get; set; }

    public int UnitPropertiesId;

    public int Executions;
}