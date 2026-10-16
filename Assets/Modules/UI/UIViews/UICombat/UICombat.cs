using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICombat : UIView
{
    [Header("TIMER")]
    [Tooltip("Padre del timer del cuarto")]
    public UIFloating floatingTimer;

    [Tooltip("Timer del cuarto")]
    public TMP_Text textTimer;

    [Header("TURN MENU")]
    [Tooltip("Padre del menu de turno.")]
    public UIFloating floatingTurnMenu;

    [Tooltip("Texto de 'Terminar' del botón del menú de turno.")]
    public TMP_Text textEnd;

    [Tooltip("Es la imagen con fill para el timer del turno.")]
    public Image imageFillTimerTurn;

    [Header("ACTIONS")]
    [Tooltip("Padre de las acciones")]
    public UIFloating floatingActions;

    /// <summary>
    /// Para recibir comandos.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUICombat:
                panel.SetActive(true);
                floatingActions.gameObject.SetActive(false);
                floatingTurnMenu.gameObject.SetActive(false);
                break;
            case GameCommand.StartTimer:
                {
                    floatingTimer.gameObject.SetActive(true);
                    int lnTime = (int)loParams[0];
                    GameManager.RunTimer(nameof(SignalRCommand.StartTimer), lnTime, UpdateTimerText);
                }
                break;
            case GameCommand.SetTurn:
                {
                    floatingActions.gameObject.SetActive(false);
                    floatingTurnMenu.gameObject.SetActive(true);
                    float lnTime = (float)loParams[0];
                    GameManager.RunTimer(nameof(UpdateTimerTurn), -lnTime, UpdateTimerTurn);
                }
                break;

            case GameCommand.SetTurnOpponent:
                {
                    floatingActions.gameObject.SetActive(false);
                    floatingTurnMenu.gameObject.SetActive(false);
                    float lnTime = (float)loParams[0];
                }
                break;

            case GameCommand.SetSelectedUnit:
                {
                    int lnUserId = (int)loParams[0];
                    BoardUnit loBoardUnit = (BoardUnit)loParams[1];


                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// Es para actualizar el fill del timer.
    /// </summary>
    /// <param name="lnDelta"></param>
    private void UpdateTimerTurn(float lnDelta)
    {
        imageFillTimerTurn.fillAmount = lnDelta;
    }

    /// <summary>
    /// Actualizamos el texto del timer.
    /// </summary>
    /// <param name="lcTime"></param>
    private void UpdateTimerText(int lnSecondsLeft, string lcTime)
    {
        textTimer.text = lcTime;
    }

    /// <summary>
    /// Se llama para indicar que deseamos entrar a la configuración.
    /// </summary>
    public void OnClickConfiguration()
    {
        
    }

    /// <summary>
    /// Se llama para indicar que terminamos el turno.
    /// </summary>
    public void OnClickEndTurn()
    {
        //<< Avisamos que deseamos terminar el turno.
        GameManager.Send(GameCommand.EndTurn);
    }
}

//Sin selección no hay nada
//Stacks de los items
//El boton vacio es configuracion
//Para salir va en la pantalla de configuración
//La vida siempre en los monos pero la energia va en el UI
//Son los buffs y debuffs con circulo con timer al click le da la información
//Icono de la clase


//Numero de Acciones pendientes
//Numero de Movimientos pendientes
//Numero de Ataques pendientes
//Que se respeta las dimensiones de las imagenes del UI o la imagen que viene de referencia
//Pasar la imagen avatar holder por elementos por separado marco avatar de unidades