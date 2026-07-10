public class UnitAssetBundle
{
    /// <summary>
    /// Indica el tipo de assetbundle.
    /// </summary>
    public UnitAssetBundleType Type;

    /// <summary>
    /// Nombre del transform al cual debe anclarse, si está vacio no se ancla como por ejemplo en el del cuerpo.
    /// </summary>
    public string AttachTransformName;

    /// <summary>
    /// Facción a la que pertenece.
    /// </summary>
    public UnitFaction Faction;

    /// <summary>
    /// Rol al que pertenece.
    /// </summary>
    public UnitRole Role;

    /// <summary>
    /// Animator de nuestro cuerpo/instancia (prefab a instanciar).
    /// </summary>
    public UnitInstance unitInstance;

    /// <summary>
    /// Root de nuestro elemento (prefab a instanciar).
    /// </summary>
    public UnitElement unitElement;
}