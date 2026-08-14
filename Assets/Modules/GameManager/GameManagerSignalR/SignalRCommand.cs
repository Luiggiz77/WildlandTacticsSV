public enum SignalRCommand : uint
{
    None = 0,
    /// <summary>
    /// Indica que pedimos el tablero de distribución que se usará.
    /// </summary>
    RequestDistributionBoardId = 1,
    /// <summary>
    /// Indica que se manda el tablero con los datos de las unidades.
    /// </summary>
    Board = 2,
}