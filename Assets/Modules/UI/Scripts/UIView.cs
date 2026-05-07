using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    [Tooltip("Panel principal para ocultar o mostrar.")]
    public GameObject panel;

    /// <summary>
    /// Awake.
    /// </summary>
    protected virtual void Awake()
    {
        GameManager.AddListener(OnGameCommand);
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    protected abstract void OnGameCommand(GameCommand loCommand,params object[] loParams);
}
