/// <summary>
/// Es el tipo de uso que se le da al texto. Ejemplo Items, Descripciones de Items, Unidades, Descripción de unidades, etc..
/// Nota: Incluso podria dividirse en UIMain, UICombat, etc...
/// </summary>
public enum GameTextUsage : int
{
    None = 0,
    /// <summary>
    /// Enum GameTextWord
    /// </summary>
    Word = 1,
    /// <summary>
    /// Enum GameTextSentence
    /// </summary>
    Sentence = 2,
    /// <summary>
    /// Enum UnitRole
    /// </summary>
    UnitRole = 3,
    /// <summary>
    /// int UnitBattlePropertiesId
    /// </summary>
    UnitDescription = 4,
    /// <summary>
    /// int UnitBattlePropertiesId
    /// </summary>
    UnitAttackName = 5,
    /// <summary>
    /// int UnitBattlePropertiesId
    /// </summary>
    UnitAttackDescription = 6,
    /// <summary>
    /// int UnitBattlePropertiesId
    /// </summary>
    UnitMovementDescription = 7,

    /// <summary>
    /// Nombre del objeto de juego.
    /// </summary>
    GameplayItemName = 8,
    /// <summary>
    /// Descripción del objeto de juego.
    /// </summary>
    GameplayItemDescription = 9,

    /// <summary>
    /// Categoria de objetos comprables.
    /// </summary>
    PurchasableCategory = 10,

    /// <summary>
    /// int PurchasableId
    /// </summary>
    PurchasableName = 11,
    /// <summary>
    /// int PurchasableId
    /// </summary>
    PurchasableDescription = 12,
    /// <summary>
    /// Son mensajes de error.
    /// </summary>
    Error = 13,
}