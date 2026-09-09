using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIView
{
    [Tooltip("Fondo de la pantalla de carga.")]
    public Image background;

    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUILoading:
                transform.SetAsLastSibling();
                panel.SetActive(true);
                background.enabled = loParams.Length > 0 && (bool)loParams[0];
                break;

            case GameCommand.HideUILoading: panel.SetActive(false); break;
            default: break;
        }
    }
}