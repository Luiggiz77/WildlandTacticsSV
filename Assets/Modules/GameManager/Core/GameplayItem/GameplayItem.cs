public class GameplayItem
{
    /// <summary>
    /// Id del objeto.
    /// </summary>
    public int Id;

    /// <summary>
    /// Id del objeto de coleccion de efectos.
    /// </summary>
    public int GameplayEffectCollectionId;

    /// <summary>
    /// Indica cuando se puede usar el objeto.
    /// </summary>
    public GameplayItemTiming Timing;

    /// <summary>
    /// Id de nuestro icono.
    /// </summary>
    public int GameTexture2DId = 0;
}