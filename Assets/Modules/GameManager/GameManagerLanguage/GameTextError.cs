using System;

[Serializable]
public enum GameTextError : int
{
    /// <summary>
    /// Mensaje generico de error.
    /// </summary>
    Error = 0,

    /// <summary>
    /// Mensaje generico de error con reintentar proceso.
    /// </summary>
    Retry = 0,
}
