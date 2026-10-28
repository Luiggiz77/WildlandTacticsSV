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

    [Header("CONTROLS")]

    [Tooltip("Portaretrato de la unidad del usuario")]
    public UICombatUnitPortrait unitPortraitUser;

    [Tooltip("Portaretrato de la unidad del oponente")]
    public UICombatUnitPortrait unitPortraitOpponent;

    [Tooltip("Stats de la unidad del usuario")]
    public UICombatUnitStats unitStatsUser;

    [Tooltip("Stats de la unidad del oponente")]
    public UICombatUnitStats unitStatsOpponent;

    [Tooltip("Panel con el botón de confirmar u (Atacar,Curar,Mover)")]
    public UIFloating confirmation;

    [Header("BUTTONS")]
    [Tooltip("Toggle de ataque.")]
    public Button buttonAttack;

    [Tooltip("Toggle de curación.")]
    public Button buttonHeal;

    [Tooltip("Toggle de movimiento.")]
    public Button buttonMove;

    [Header("IMAGES")]
    [Tooltip("Imagen de ataque.")]
    public Image imageAttack;

    [Tooltip("Imagen de curación.")]
    public Image imageHeal;

    [Tooltip("Imagen de movimiento.")]
    public Image imageMove;

    [Header("SPRITES")]
    [Tooltip("Sprite de ataque On.")]
    public Sprite spriteAttackOn;

    [Tooltip("Sprite de ataque Off.")]
    public Sprite spriteAttackOff;

    [Tooltip("Sprite de curación On.")]
    public Sprite spriteHealOn;

    [Tooltip("Sprite de curación Off.")]
    public Sprite spriteHealOff;

    [Tooltip("Sprite de movimiento On.")]
    public Sprite spriteMoveOn;

    [Tooltip("Sprite de movimiento Off.")]
    public Sprite spriteMoveOff;

    [Header("Text")]
    [Tooltip("Texto botón terminar turno.")]
    public TMP_Text textButtonEndTurn;
    [Tooltip("Texto botón confirmar acción.")]
    public TMP_Text textButtonConfirm;
    [Tooltip("Texto botón cancelar acción.")]
    public TMP_Text textButtonCancel;

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
                    float lnTime = (float)loParams[0];
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

                    //<< Revisamos si es del usuario u oponente.
                    if (loBoardUnit.UserId == lnUserId)
                    {
                        unitStatsUser.UpdateUnitStats(loBoardUnit);
                        unitStatsUser.gameObject.SetActive(true);
                        unitPortraitUser.gameObject.SetActive(true);
                        floatingActions.gameObject.SetActive(floatingTurnMenu.gameObject.activeSelf);
                    }
                    else
                    {
                        unitStatsOpponent.UpdateUnitStats(loBoardUnit);
                        unitStatsOpponent.gameObject.SetActive(true);
                        unitPortraitOpponent.gameObject.SetActive(true);
                    }
                }
                break;

            case GameCommand.UICombat_DisplayActions:
                {
                    DeactivateActionButtons();
                    floatingActions.gameObject.SetActive((bool)loParams[0]);
                }
                break;

            case GameCommand.UICombat_DisplayActionHeal: buttonHeal.gameObject.SetActive((bool)loParams[0]); break;

            case GameCommand.UICombat_EnableActionAttack:
                {
                    bool lbEnable = (bool)loParams[0];
                    buttonAttack.interactable = lbEnable;
                    imageAttack.overrideSprite = lbEnable ? spriteAttackOn : spriteAttackOff;
                }
                break;

            case GameCommand.UICombat_EnableActionMove:
                {
                    bool lbEnable = (bool)loParams[0];
                    buttonMove.interactable = lbEnable;
                    imageMove.overrideSprite = lbEnable ? spriteMoveOn : spriteMoveOff;
                }
                break;

            case GameCommand.UICombat_EnableActionHeal:
                {
                    bool lbEnable = (bool)loParams[0];
                    buttonHeal.interactable = lbEnable;
                    imageHeal.overrideSprite = lbEnable ? spriteHealOn : spriteHealOff;
                }
                break;

            case GameCommand.UICombat_DisplayConfirmation:
                {
                    confirmation.gameObject.SetActive((bool)loParams[0]);
                    UnitActionMode loUnitActionMode = (UnitActionMode)loParams[1];
                    switch (loUnitActionMode)
                    {
                        case UnitActionMode.Move: textButtonConfirm.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Move); break;
                        case UnitActionMode.Attack: textButtonConfirm.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Attack); break;
                        case UnitActionMode.Heal: textButtonConfirm.text = GameManager.GetText(GameTextUsage.Word, GameTextWord.Heal); break;
                        default: textButtonConfirm.text = string.Empty; break;
                    }
                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// Se llama en los eventos de mouse del panel que son llevados al tablero.
    /// </summary>
    public void OnBoardMouseDown()
    {
        GameManager.Send(GameCommand.OnBoardMouseDown);
    }

    /// <summary>
    /// Se llama en los eventos de mouse del panel que son llevados al tablero.
    /// </summary>
    public void OnBoardMouseDrag()
    {
        GameManager.Send(GameCommand.OnBoardMouseDrag);
    }

    /// <summary>
    /// Se llama en los eventos de mouse del panel que son llevados al tablero.
    /// </summary>
    public void OnBoardMouseUp()
    {
        GameManager.Send(GameCommand.OnBoardMouseUp);
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

    /// <summary>
    /// Se llama cuando le dan click al boton de confirmar la acción, ya sea para ataque, curacion o movimiento.
    /// </summary>
    public void OnClickConfirmAction()
    {
        GameManager.Send(GameCommand.ConfirmUnitAction);
    }

    /// <summary>
    /// Se llama para cuando le dan click al boton de cancelar la acción.
    /// </summary>
    public void OnClickCancelAction()
    {
        GameManager.Send(GameCommand.CancelUnitAction);
        confirmation.gameObject.SetActive(false);
    }

    /// <summary>
    /// Se usa para desactivar los botones de las acciones.
    /// </summary>
    private void DeactivateActionButtons()
    {
        buttonMove.interactable = false;
        buttonAttack.interactable = false;
        buttonHeal.interactable = false;
        imageMove.overrideSprite = spriteMoveOff;
        imageAttack.overrideSprite = spriteAttackOff;
        imageHeal.overrideSprite = spriteHealOff;
        confirmation.gameObject.SetActive(false);
    }

    /// <summary>
    /// Para cuando moveremos la unidad.
    /// </summary>
    public void OnClickMove()
    {
        GameManager.Send(GameCommand.SetUnitActionMode, UnitActionMode.Move);
    }

    /// <summary>
    /// Para cuando atacaremos con la unidad.
    /// </summary>
    public void OnClickAttack()
    {
        GameManager.Send(GameCommand.SetUnitActionMode, UnitActionMode.Attack);
    }

    /// <summary>
    /// Para cuando curaremos con la unidad.
    /// </summary>
    public void OnClickHeal()
    {
        GameManager.Send(GameCommand.SetUnitActionMode, UnitActionMode.Heal);
    }

    /// <summary>
    /// Para cuando usaremos objetos.
    /// </summary>
    public void OnClickItems()
    {
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


//Que sucede cuando se intercambia el turno en las acciones? Se reinicia?
//Siempre se bloquean las acciones cuando no hay acciones o ataques por realizar?
//La curación se bloquea con base en la energia? Check de curación o efecto que da vida?
//Si es cero se bloquea la curación, se bloquean klos dos botónes