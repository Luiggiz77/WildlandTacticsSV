using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDownloading : UIView
{
    [Tooltip("Barra de porcentaje.")]
    public Image loadingBar;

    [Tooltip("Texto de la pantalla de descarga.")]
    public TMP_Text text;

    /// <summary>
    /// Awake
    /// </summary>
    protected override void Awake()
    {
        loadingBar.fillAmount = 0.0f;
        text.text = string.Empty;
        base.Awake();
    }

    /// <summary>
    /// OnDisable.
    /// </summary>
    private void OnDisable()
    {
        loadingBar.fillAmount = 0.0f;
        text.text = string.Empty;
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUIDownloading: panel.SetActive(true); break;
            case GameCommand.HideUIDownloading: panel.SetActive(false); break;
            case GameCommand.SetUIDownloadingPercent: loadingBar.fillAmount = (float)loParams[0]; break;
            case GameCommand.SetUIDownloadingText: text.text = (string)loParams[0]; break;
            default: break;
        }
    }
}