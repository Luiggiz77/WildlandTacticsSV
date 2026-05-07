using System;

[Serializable]
public enum GameTextSentence : int
{
    None = 0,
    /// <summary>
    /// Indica que el rogue se ha movido.
    /// </summary>
    RogueMoved = 1,
    /// <summary>
    /// Indica que hemos recivido un objeto.
    /// </summary>
    ObjectReceived = 2,
    /// <summary>
    /// Es el texto de confirmación para comprar un objeto del juego.
    /// </summary>
    BuyGameplayItem = 3,
    /// <summary>
    /// Es el texto que nos pregunta si deseamos terminar el turno.
    /// </summary>
    EndTurnConfirmation = 4,
}