public enum UnitActionState : int
{
    /// <summary>
    /// Indica que estamos seleccionando/eligiendo unidad y el modo de acción.
    /// </summary>
    SelectingUnit = 0,
    /// <summary>
    /// Indica que estamos seleccionando las celdas para realizar la acción en ellas.
    /// </summary>
    SelectingCells = 1,
    /// <summary>
    /// Indica que estamos por confirmar la acción de la unidad seleccionada.
    /// </summary>
    ConfirmingAction = 2,
}
