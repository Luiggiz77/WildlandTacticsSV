public class UnitEventInstruction
{
    /// <summary>
    /// Comando a ejecutar.
    /// </summary>
    public ClientCommand Command;

    /// <summary>
    /// Id del assetbundle que se usa en caso de ser así.
    /// </summary>
    public int? VisualEffectAssetBundleId;

    /// <summary>
    /// Id del efecto a reproducir en caso de ser requerido por la instrucción "ClientCommand".
    /// </summary>
    public int? GameplayEffectId;

    /// <summary>
    /// Valor del comando que debe aplicarse.
    /// </summary>
    public string Value;

    /// <summary>
    /// Indica si la instruccion se aplica al objetivo.
    /// </summary>
    public bool OnTarget = false;

    /// <summary>
    /// Mascara para saber a quien se manda la instrucción, se asigna el GameUserId si es exclusivo o 0 si no es esclusivo.
    /// </summary>
    private int gnMask = 0;

    public int GetMask() { return gnMask; }
    public void SetMask(int lnMask) { gnMask = lnMask; }

    /// <summary>
    /// Efecto de la instrucción.
    /// </summary>
    public GameplayEffect GameplayEffect;
}
