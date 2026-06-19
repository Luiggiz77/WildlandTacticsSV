using TMPro;
using UnityEngine;

public class UIMainMenu : UIView
{
    [Header("Text")]

    [Tooltip("Texto botón individual.")]
    public TMP_Text textButtonSinglePlayer;
    [Tooltip("Texto botón multijugador.")]
    public TMP_Text textButtonMultiplayer;
    [Tooltip("Texto botón plaza.")]
    public TMP_Text textButtonSquare;
    [Tooltip("Texto botón escuadrón.")]
    public TMP_Text textButtonSquad;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// Se llama para actualizar los textos.
    /// </summary>
    private void UpdateText()
    {
        textButtonSinglePlayer.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.SinglePlayer);
        textButtonMultiplayer.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Multiplayer);
        textButtonSquare.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Square);
        textButtonSquad.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Squad);
    }

    /// <summary>
    /// Se llama cuando la dan click al botón de un solo jugador.
    /// </summary>
    public void OnClickSinglePlayer()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUISinglePlayer);
    }

    /// <summary>
    /// Se llama cuando la dan click al botón de multijugador.
    /// </summary>
    public void OnClickMultiPlayer()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUIMultiplayer);
    }

    /// <summary>
    /// Se llama cuando la dan click al botón de campamento.
    /// </summary>
    public void OnClickCamp()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUICamp);
    }

    /// <summary>
    /// Se llama cuando la dan click al botón de carnet.
    /// </summary>
    public void OnClickLicense()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUILicense);
    }

    /// <summary>
    /// Se llama cuando le dan click a configuración.
    /// </summary>
    public void OnClickSettings()
    {
        //panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUISettings);
    }

    /// <summary>
    /// Se llama cuando le dan click a configuración.
    /// </summary>
    public void OnClickBattlePreparation()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUICombatPreparation);
    }

    /// <summary>
    /// Se llama cuando le dan click a la plaza
    /// </summary>
    public void OnClickMarket()
    {
        panel.gameObject.SetActive(false);
        GameManager.Send(GameCommand.ShowUISquare);
    }

    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUIMainMenu: panel.SetActive(true); break;
            case GameCommand.LanguageChanged: UpdateText(); break;
            default: break;
        }
    }
}
