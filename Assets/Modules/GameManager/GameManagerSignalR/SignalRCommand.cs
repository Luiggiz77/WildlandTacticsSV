public enum SignalRCommand : uint
{
    None = 0,
    /// <summary>
    /// Indicamos que debemos reiniciar flujo por algún timeout.
    /// </summary>
    Restart = 1,
    /// <summary>
    /// Indica que pedimos el tablero de distribución que se usará.
    /// </summary>
    RequestDistributionBoardId = 2,
    /// <summary>
    /// Indica que se manda el tablero con los datos de las unidades.
    /// </summary>
    Board = 3,
    /// <summary>
    /// Le preguntamos al usuario si le damos mas tiempo al oponente.
    /// </summary>
    WaitForOpponent = 4,
    /// <summary>
    /// Indica que debemos mostrar el tablero y pantalla de interfaz sin la sección de acciones en ambos jugadores.
    /// </summary>
    ShowBoard = 5,
    /// <summary>
    /// Es para indicar de quien es el turno.
    /// </summary>
    SetTurn = 6,
    /// <summary>
    /// Indica que quiere salir voluntariamente.
    /// </summary>
    Quit = 7,
}