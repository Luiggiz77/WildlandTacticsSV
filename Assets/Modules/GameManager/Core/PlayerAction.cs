using System;
using System.Collections.Generic;

[Serializable]
public class PlayerAction
{
    public enum ActionType : int
    {
        None = 0,
        /// <summary>
        /// Indica que moverá una unidad.
        /// </summary>
        Move = 1,
        /// <summary>
        /// Indica que atacará con una unidad.
        /// </summary>
        Attack = 2,
        /// <summary>
        /// Indica que usará un item.
        /// </summary>
        Item = 3,
    }

    /// <summary>
    /// Nos indica la acción a realizar.
    /// </summary>
    public ActionType Action { get; set; }

    /// <summary>
    /// Es el Id de la unidad que realizará la acción.
    /// </summary>
    public int UnitPropertiesId { get; set; } = 0;

    /// <summary>
    /// Son las celdas involucradas en la acción.
    /// </summary>
    public List<int> CellIndexes { get; set; } = new List<int>();
}