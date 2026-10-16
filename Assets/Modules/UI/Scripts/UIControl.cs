using UnityEngine;

public abstract class UIControl : MonoBehaviour
{
    /// <summary>
    /// OnEnable.
    /// </summary>
    protected virtual void OnEnable()
    {
        GameManager.AddListener(OnGameCommand);
    }

    /// <summary>
    /// OnDisable.
    /// </summary>
    protected virtual void OnDisable()
    {
        GameManager.RemoveListener(OnGameCommand);
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected abstract void OnGameCommand(GameCommand loCommand, params object[] loParams);
}
