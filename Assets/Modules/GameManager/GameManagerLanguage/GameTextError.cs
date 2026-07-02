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
    Retry = 1,

    /// <summary>
    /// Error de que no hay conexión a internet.
    /// </summary>
    NoInternet = 2,

    /// <summary>
    /// Indica que se ha llegado al maximo de unidades.
    /// </summary>
    MaxUnits = 2,

}
