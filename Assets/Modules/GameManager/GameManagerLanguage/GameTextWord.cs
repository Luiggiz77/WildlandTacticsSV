using System;

[Serializable]
public enum GameTextWord : int
{
    None = 0,
    /// <summary>
    /// Individual
    /// </summary>
    SinglePlayer = 1,
    /// <summary>
    /// Multijugador.
    /// </summary>
    Multiplayer = 2,
    /// <summary>
    /// Plaza
    /// </summary>
    Square = 3,
    /// <summary>
    /// Escuadrón
    /// </summary>
    Squad = 4,
    /// <summary>
    /// Preparación
    /// </summary>
    Preparation = 5,
    /// <summary>
    /// Objetos
    /// </summary>
    Objects = 6,
    /// <summary>
    /// Iniciar Batalla
    /// </summary>
    StartBattle = 7,
    /// <summary>
    /// Atacar
    /// </summary>
    Attack = 8,
    /// <summary>
    /// Mover
    /// </summary>
    Move = 9,
    /// <summary>
    /// Objetos
    /// </summary>
    Items = 10,
    /// <summary>
    /// Terminar Turno
    /// </summary>
    EndTurn = 11,
    /// <summary>
    /// Confirmar.
    /// </summary>
    Confirm = 12,
    /// <summary>
    /// Configuración.
    /// </summary>
    Settings = 13,
    /// <summary>
    /// Notificaciones.
    /// </summary>
    Notifications = 14,
    /// <summary>
    /// Si
    /// </summary>
    Yes = 15,
    /// <summary>
    /// No
    /// </summary>
    No = 16,
    /// <summary>
    /// Regresar
    /// </summary>
    Return = 17,
    /// <summary>
    /// Usar.
    /// </summary>
    Use = 18,
    /// <summary>
    /// Cancelar
    /// </summary>
    Cancel = 19,
    /// <summary>
    /// Letra que indica "informacion", como la "i" o como "?"
    /// </summary>
    InformationLetter = 20,
    /// <summary>
    /// Mis escuadrones
    /// </summary>
    MySquad = 21,
    /// <summary>
    /// Movimiento
    /// </summary>
    Movement = 22,
    /// <summary>
    /// Nombre de la moneda.
    /// </summary>
    CurrencyName = 23,
}