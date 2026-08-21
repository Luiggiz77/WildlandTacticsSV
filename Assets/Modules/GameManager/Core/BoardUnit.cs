using System;
using System.Collections.Generic;

[Serializable]
public class BoardUnit
{
    /// <summary>
    /// Indica la vida maxima actual de la unidad.
    /// </summary>
    public int HealthMax = 0;

    /// <summary>
    /// Indica la vida actual de la unidad.
    /// </summary>
    public int Health = 0;

    /// <summary>
    /// Indica el nivel de energia de la unidad.
    /// </summary>
    public int Energy = 0;

    /// <summary>
    /// Indica la cantidad de ataques que ha realizado la unidad.
    /// </summary>
    public int Attacks = 0;

    /// <summary>
    /// Indica la cantidad de movimientos que ha realizado la unidad.
    /// </summary>
    public int Moves = 0;

    /// <summary>
    /// Direccion en la que ve la unidad.
    /// </summary>
    public Facing Facing;

    public UnitProperties UnitProperties;

    public List<UnitGameplayEffect> UnitGameplayEffects = new List<UnitGameplayEffect>();
}