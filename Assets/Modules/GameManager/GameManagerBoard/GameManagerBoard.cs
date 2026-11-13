using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public partial class GameManager : MonoBehaviour
{
    private static Quaternion Forward = Quaternion.identity;
    private static Quaternion Backward = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private static Quaternion Left = Quaternion.Euler(0.0f, -90.0f, 0.0f);
    private static Quaternion Right = Quaternion.Euler(0.0f, 90.0f, 0.0f);

    /// <summary>
    /// Tablero del juego que se asigna en SignalR.
    /// </summary>
    private static Board<BoardUnit> goBoard;

    [Header("BOARD")]
    [Tooltip("Elemento que contendrá todos los prefabs.")]
    public Transform boardParent;

    [Tooltip("Elemento que se usa como linea para delimitar el mapa.")]
    public Transform gridLine;

    [Tooltip("Elemento que se usa como checker.")]
    public Transform checker;

    [Tooltip("Es el recuadro que aparece en los espacios del tablero vacios al ser seleccionados.")]
    public Transform boardCellSelectedEmpty;

    [Tooltip("Es el prefab que se usa para indicar las celdas a las que la unidad se puede mover.")]
    public Transform prefabBoardCellMove;

    [Tooltip("Es el prefab que se usa para indicar las celdas a las que la unidad puede atacar.")]
    public Transform prefabBoardCellAttack;

    [Tooltip("Es el prefab que se usa para indicar las celdas por las que la unidad se moverá/atacará.")]
    public Transform prefabBoardCellSelected;

    [Tooltip("Es el prefab que se usa para indicar las celdas del area de efecto.")]
    public Transform prefabBoardCellEffectArea;

    [Tooltip("Es el prefab que se usa para los recuadros del tablero para poder usar el raycast en dichos colliders.")]
    public Transform prefabBoardCellSelector;

    [Tooltip("Prefab de la barra de vida de las unidades. (Temp)")]
    public UnitHealthbar prefabUnitHealthbar;

    [Header("COLORS")]
    [Tooltip("Color de la barra de vida del usuario")]
    public Color HealthbarColor;

    [Tooltip("Outline material user.")]
    public Color unitOutlineColorUser = Color.aliceBlue;

    [Tooltip("Outline material opponent.")]
    public Color unitOutlineColorSecundary = Color.orangeRed;

    [Header("CAMERA")]
    [Tooltip("Es la camara que muestra el loop de ataque.")]
    public GameObject cameraUnitAttackLoop;

    [Tooltip("Es la camara que muestra el portraretrato de la unidad del usuario.")]
    public GameObject cameraUnitPortraitPrimary;

    [Tooltip("Es la camara que muestra el portraretrato de la unidad secundaria.")]
    public GameObject cameraUnitPortraitSecondary;

    [Header("Theater animations")]
    public AnimationCurve theaterAnimationCurve;

    /// <summary>
    /// Nuestra celda de unidad seleccionada.
    /// </summary>
    private int gnSelectedCellIndex = -1;

    /// <summary>
    /// Nos indica el estado de la acción de la unidad.
    /// </summary>
    private UnitActionState goUnitActionState = UnitActionState.SelectingUnit;

    /// <summary>
    /// Nos indica el modo de accion que se encuentra la unidad seleccionada.
    /// </summary>
    private UnitActionMode goUnitActionMode = UnitActionMode.None;

    /// <summary>
    /// Son las propiedades del tablero.
    /// </summary>
    private BoardProperties goBoardProperties = new BoardProperties();

    /// <summary>
    /// Lista de nuestras lineas de borde del tablero.
    /// </summary>
    private static List<Transform> goGridLinesBorder = new List<Transform>();

    /// <summary>
    /// Lista de nuestras lineas de tablero.
    /// </summary>
    private static List<Transform> goGridLines = new List<Transform>();

    /// <summary>
    /// Lista de nuestros elementos que conforman el checker del tablero.
    /// </summary>
    private static List<Transform> goCheckers = new List<Transform>();

    /// <summary>
    /// Se usa para obtener las celdas por las que puede moverse la unidad y a las que puede dirigir el ataque.
    /// </summary>
    private static List<BoardCell<BoardUnit>> goUnitSurroundingCells = new List<BoardCell<BoardUnit>>();

    /// <summary>
    /// Se usa para almacenar el path/celda seleccionado por el usuario.
    /// </summary>
    private static List<BoardCell<BoardUnit>> goUnitCellsSelected = new List<BoardCell<BoardUnit>>();

    /// <summary>
    /// Se usa para almacenar el area de efecto.
    /// </summary>
    private static List<BoardCell<BoardUnit>> goCellsEffectArea = new List<BoardCell<BoardUnit>>();

    /// <summary>
    /// Ray
    /// </summary>
    private Ray goRay;

    /// <summary>
    /// RaycastHit
    /// </summary>
    private RaycastHit goRaycastHit;

    /// <summary>
    /// Se usa para indicar la posicion normalizada con base en el path usando el campo "goUnitCellPaths".
    /// Nota: Tambien nos indica que debemos desplazar nuestra unidad seleccionada si es mayor a 0.
    /// Nota: Cuando se quiere iniciar un desplazamiento de unidad debemos tener un path y asignar este campo el valor de 0.
    /// </summary>
    private float gnDeltaCellPath = -1.0f;

    /// <summary>
    /// Magnitud de la posicion de nuestro mouse.
    /// </summary>
    private float gnMousePositionMagnitude = 0.0f;

    /// <summary>
    /// Es la ultima unidad del tablero seleccionada del usuario.
    /// </summary>
    private BoardUnit goSelectedBoardUnit = null;

    /// <summary>
    /// Es la ultima unidad del tablero seleccionada del oponente.
    /// </summary>
    private BoardUnit goSelectedBoardUnitSecondary = null;

    /// <summary>
    /// Es el clon de la unidad usada para el portaretrato.
    /// </summary>
    private UnitInstance goUnitInstancePortraitUser = null;

    /// <summary>
    /// Es el clon de la unidad usada para el portaretrato.
    /// </summary>
    private UnitInstance goUnitInstancePortraitSecondary = null;

    private static MaterialPropertyBlock goMPB;

    /// <summary>
    /// Se usa para guardar/cargar una posicion atraves de comandos ya sea "UnitEventCommand" o "GameplayEffectCommand".
    /// </summary>
    private static Vector3 goStoredPosition = Vector3.zero;

    /// <summary>
    /// Se usa para guardar/cargar un transform atraves de comandos ya sea "UnitEventCommand" o "GameplayEffectCommand".
    /// </summary>
    private static Transform goStoredTransform = null;

    /// <summary>
    /// Indica la cantidad de celdas que se desplaza la unidad al ser empujada es un valor almacenado al llamar el GameplayEffectType.InvokeUnitEventPushAsync.
    /// </summary>
    private int gnPushCells = 1;

    /// <summary>
    /// Indica la velocidad de traslación del empuje almacenada al llamar el GameplayEffectType.InvokeUnitEventPushAsync.
    /// </summary>
    private float gnPushSpeed = 1.0f;

    /// <summary>
    /// Indica la cantidad de veces que se llama un callback (por el momento se usa solo al empujar unidades).
    /// </summary>
    private static int gnCallbackCount = 0;

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    private void AwakeBoard()
    {
        //<< Nos conectamos a los comandos.
        AddListener(OnGameCommandBoard);

        goMPB = new MaterialPropertyBlock();
    }

    private static void SetBoardElementColor(Component loComponent, Color loColor)
    {
        Renderer loRenderer = loComponent.gameObject.GetComponentInChildren<Renderer>();
        loRenderer.GetPropertyBlock(goMPB);
        goMPB.SetColor("_BaseColor", loColor);
        goMPB.SetColor("_EmissionColor", loColor);
        loRenderer.SetPropertyBlock(goMPB);
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommandBoard(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.OnBoardMouseDrag: OnBoardMouseDrag(); break;
            case GameCommand.OnBoardMouseUp: OnBoardMouseUp(); break;
            case GameCommand.SetUnitActionMode: OnSetActionMode((UnitActionMode)loParams[0]); break;
            case GameCommand.CancelUnitAction: DeactivateCellsSelected(); break;
            case GameCommand.ReturnActionState: OnReturnActionState(); break;
            default: break;
        }
    }

    /// <summary>
    /// Se asigna el tablero lo cual nos indica que debemos crear todos los elementos del mismo.
    /// </summary>
    /// <param name="loBoard"></param>
    private void SetBoard(Board<BoardUnit> loBoard)
    {
        goBoard = loBoard;
        goBoard.ConnectBoard();
        goBoard.CreateSelectedCells();

        //<< Creamos el grid del tablero.
        CreateGridAndChecker(loBoard.Width, loBoard.Lenght);

        //<< Creamos las celdas de selección.
        CreateSelectionCells(goBoardProperties.boardWidht, goBoardProperties.boardLenght);

        Dictionary<int, UnitFaction> loFactions = new Dictionary<int, UnitFaction>();
        loFactions.Add(0, UnitFaction.Any);
        loFactions.Add(loBoard.UserIdA, loBoard.FactionUserA);
        loFactions.Add(loBoard.UserIdB, loBoard.FactionUserB);

        //<< Instanciamos y posicionamos las unidades del tablero.
        UnitInstance loUnitInstance;
        UnitFaction loFaction;
        BoardCell<BoardUnit>[] loCells = loBoard.GetCells();
        foreach (BoardCell<BoardUnit> loCell in loCells)
        {
            foreach (BoardUnit loBoardUnit in loCell.Data)
            {
                if (!loFactions.TryGetValue(loBoardUnit.UserId, out loFaction)) loFaction = UnitFaction.Any;

                //<< Creamos la unidad con base en sus elementos visuales.
                loUnitInstance = CreateUnitInstance(loBoardUnit.UnitProperties, loFaction);

                //<< Establecemos padre y posicion.
                loUnitInstance.transform.SetParent(boardParent);
                loUnitInstance.transform.position = new Vector3(loCell.X, 0.0f, loCell.Z);
                loUnitInstance.transform.rotation = loBoardUnit.Facing.ToQuaternion();

                //<< Creamos su barra de vida si lo requiere.
                if (!loBoardUnit.UnitProperties.UnitBattleProperties.Healthbar) continue;

                int lnHashCodeUnitHealtbar = prefabUnitHealthbar.GetHashCode();
                UnitHealthbar loUnitHealthbar = Spawn(lnHashCodeUnitHealtbar, prefabUnitHealthbar);
                loUnitHealthbar.transform.SetParent(loUnitInstance.transform);
                loUnitHealthbar.transform.localPosition = Vector3.up * 1.5f;
                loUnitHealthbar.SetUnitProperties(loBoardUnit.UnitProperties);
                loUnitHealthbar.SetMax(loBoardUnit.HealthMax);
                loUnitHealthbar.SetCount(loBoardUnit.Health);
            }
        }
    }

    /// <summary>
    /// Nos indican que debemos reproducir las intrucciones que vienen en el tablero y reparar tablero de ser necesario con la actualización que mandan.
    /// </summary>
    /// <param name="loBoard"></param>
    private IEnumerator PlayBoardInstructions(Board<BoardUnit> loBoard)
    {
        //<< Reproduzco coroutine y espero a que terminen las instrucciones, despues actualizo tablero.
        //<< TODO: Revisamos si la seleccion es correcta en servidor. (loCellIds)

        UnitEventInstruction loUnitEventInstruction;
        int lnCount = loBoard.Instructions.Count;
        for (int i = 0; i < lnCount; i++)
        {
            loUnitEventInstruction = loBoard.Instructions[i];
            switch (loUnitEventInstruction.Command)
            {
                default: break;
            }
        }



        ////<< Obtenemos la unidad seleccionada.
        //UnitProperties loUnitProperties = loBoard. GetUnitStoredProperties(lnUnitStoredPropertiesId);
        //loUnitProperties.Attacks--;

        ////<< Aplicamos el evento de ataque.
        //Facing loFacing = loUnitProperties.GetFacing(loBoardCell.X, loBoardCell.Z);
        //yield return ApplyEvent(UnitEventTiming.Attack, loUnitProperties, loUnitProperties, loBoardCell.Index, loFacing);

        ////<< Indicamos que terminamos la acción.
        //yield return UnitEndedAction(loUnitProperties);
        yield break;
    }

    /// <summary>
    /// Se llama para reproducir las instrucciones de un evento/efecto.
    /// </summary>
    private IEnumerator PlayEvent(UnitEventTiming loUnitEventTiming, BoardUnit loBoardUnitCaller, BoardUnit loBoardUnit, int lnCellIndexTarget, Facing loFacingTarget, List<UnitEventInstruction> loInstructions, int lnInstructionsCount, int lnInstructionIndex = 0, GameplayEffect loGameplayEffectCaller = null)
    {
        Dictionary<int, PlayablePrefab> loPlayablePrefabs = new Dictionary<int, PlayablePrefab>();
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        UnitEventInstruction loInstruction;
        int lnAnimationTrigger = 0;
        GameplayEffect loGameplayEffect = null;
        int lnGameplayEffectAreaCells = 0;
        int lnGameplayEffectAreaSection = 0;
        int lnGameplayEffectAreaSections = 0;
        int lnAnimationFlag = loBoardUnit.UnitInstance.GetAnimationFlag();
        BoardCell<BoardUnit>[] loGameplayEffectAreaCells = null;
        BoardCell<BoardUnit>[] loGameplayEffectAreaSectionCells = null;
        List<StoredVector> loStoredVectors = new List<StoredVector>();
        Renderer loBillboardIcon = null;

        //<< Iniciamos flujo de intrucciones.
        for (int i = lnInstructionIndex; i < lnInstructionsCount; i++)
        {
            loInstruction = loInstructions.ElementAt(i);
            switch (loInstruction.Command)
            {
                case ClientCommand.PlayAnimation:
                case ClientCommand.PlayAnimationWithRootMotion:
                    if (!string.IsNullOrWhiteSpace(loInstruction.Value)) lnAnimationTrigger = Animator.StringToHash(loInstruction.Value);
                    else
                    {
                        //<< Obtenemos el trigger de la animación con base en el tipo de evento.
                        switch (loUnitEventTiming)
                        {
                            //<< Push en direccion y luego CollideEmitter y CollideReceiver
                            case UnitEventTiming.Push:
                                //<< Iniciamos animación de recepcion de daño con base en nuestra dirección y la dirección del ataque.
                                if (loBoardUnit.Facing.FromFront(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.PushBack;
                                else if (loBoardUnit.Facing.FromBack(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.PushFront;
                                else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.PushLeft;
                                else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.PushRight;
                                else lnAnimationTrigger = AnimationControllerData.Triggers.PushFront;
                                break;

                            case UnitEventTiming.Move: gnDeltaCellPath = 0; loBoardUnit.UnitInstance.OnUnitAnimatorMove = OnUnitAnimatorMoveOnCells; lnAnimationTrigger = AnimationControllerData.Triggers.Move; break;
                            case UnitEventTiming.CollideEmitter:
                                {
                                    lnAnimationTrigger = AnimationControllerData.Triggers.KnockBack;

                                    //<< Si estabamos en posicion de descanso.
                                    if (loBoardUnit.UnitInstance.OnUnitAnimatorMove == null)
                                    {
                                        Vector3 loUnitPosition = loBoardUnit.UnitInstance.transform.position;

                                        //<< Si estamos desplazados en direccion Z+
                                        if (loUnitPosition.z > loBoardUnit.Z + 0.25f)
                                        {
                                            switch (loBoardUnit.Facing)
                                            {
                                                case Facing.ZPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockBack; break;
                                                case Facing.ZNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockFront; break;
                                                case Facing.XPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockRight; break;
                                                case Facing.XNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockLeft; break;
                                                default: break;
                                            }
                                        }
                                        //<< Si estamos desplazados en direccion Z-
                                        else if (loUnitPosition.z < loBoardUnit.Z - 0.25f)
                                        {
                                            switch (loBoardUnit.Facing)
                                            {
                                                case Facing.ZPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockFront; break;
                                                case Facing.ZNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockBack; break;
                                                case Facing.XPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockLeft; break;
                                                case Facing.XNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockRight; break;
                                                default: break;
                                            }
                                        }
                                        //<< Si estamos desplazados en direccion X+
                                        else if (loUnitPosition.x > loBoardUnit.X + 0.25f)
                                        {
                                            switch (loBoardUnit.Facing)
                                            {
                                                case Facing.ZPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockLeft; break;
                                                case Facing.ZNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockRight; break;
                                                case Facing.XPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockBack; break;
                                                case Facing.XNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockFront; break;
                                                default: break;
                                            }
                                        }
                                        //<< Si estamos desplazados en direccion X-
                                        else if (loUnitPosition.x < loBoardUnit.X - 0.25f)
                                        {
                                            switch (loBoardUnit.Facing)
                                            {
                                                case Facing.ZPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockRight; break;
                                                case Facing.ZNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockLeft; break;
                                                case Facing.XPos: lnAnimationTrigger = AnimationControllerData.Triggers.KnockFront; break;
                                                case Facing.XNeg: lnAnimationTrigger = AnimationControllerData.Triggers.KnockBack; break;
                                                default: break;
                                            }
                                        }
                                    }
                                    //<< else: Si veniamos caminando.

                                    loBoardUnit.UnitInstance.OnUnitAnimatorMove = OnUnitAnimatorMoveManually;
                                }
                                break;
                            case UnitEventTiming.Attack: lnAnimationTrigger = AnimationControllerData.Triggers.Attack; break;
                            case UnitEventTiming.ReceiveDamage:
                            case UnitEventTiming.CollideReceiver:
                                if (loGameplayEffectCaller == null || loGameplayEffectCaller.Type != GameplayEffectType.UnitHealthNoAnimation)
                                {
                                    //<< Revisamos si nos estamos moviendo, de ser así aplicamos mascara, de lo contrario aplicamos la animacion tal cual.
                                    if (loBoardUnit.UnitInstance.OnUnitAnimatorMove != null)
                                    {
                                        //<< Iniciamos animación de recepcion de daño con base en nuestra dirección y la dirección del ataque.
                                        if (loBoardUnit.Facing.FromBack(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageMaskBack;
                                        else if (loBoardUnit.Facing.FromFront(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageMaskFront;
                                        else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageMaskLeft;
                                        else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageMaskRight;
                                        else lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageMaskFront;
                                    }
                                    else
                                    {
                                        //<< Iniciamos animación de recepcion de daño con base en nuestra dirección y la dirección del ataque.
                                        if (loBoardUnit.Facing.FromBack(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageBack;
                                        else if (loBoardUnit.Facing.FromFront(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageFront;
                                        else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageLeft;
                                        else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageRight;
                                        else lnAnimationTrigger = AnimationControllerData.Triggers.ReceiveDamageFront;
                                    }
                                }
                                break;
                            case UnitEventTiming.Death: lnAnimationTrigger = AnimationControllerData.Triggers.Death; break;
                            case UnitEventTiming.Creation: lnAnimationTrigger = AnimationControllerData.Triggers.Creation; break;
                            default: break;
                        }
                    }

                    //<< Si nos indican que se mueve usando la animación activamos el movimiento.
                    if (loInstruction.Command == ClientCommand.PlayAnimationWithRootMotion) loBoardUnit.UnitInstance.OnUnitAnimatorMove = OnUnitAnimatorMove;

                    if (lnAnimationTrigger != 0) loBoardUnit.UnitInstance.animator.SetTrigger(lnAnimationTrigger);
                    break;

                case ClientCommand.ForceAnimation:
                case ClientCommand.ForceAnimationWithRootMotion:
                    {
                        //<< Reiniciamos bandera de animación.
                        loBoardUnit.UnitInstance.DeactivateAnimationFlag();
                        lnAnimationFlag = loBoardUnit.UnitInstance.GetAnimationFlag();

                        string lcAnimationState = string.Empty;

                        if (!string.IsNullOrWhiteSpace(loInstruction.Value)) lcAnimationState = loInstruction.Value;
                        else
                        {
                            //<< Obtenemos el trigger de la animación con base en el tipo de evento.
                            switch (loUnitEventTiming)
                            {
                                //<< Push en direccion y luego CollideEmitter y CollideReceiver
                                case UnitEventTiming.Push:
                                    //<< Iniciamos animación de recepcion de daño con base en nuestra dirección y la dirección del ataque.
                                    if (loBoardUnit.Facing.FromFront(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.PushBack);
                                    else if (loBoardUnit.Facing.FromBack(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.PushFront);
                                    else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.PushLeft);
                                    else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.PushRight);
                                    else lcAnimationState = nameof(AnimationControllerData.Triggers.PushFront);
                                    break;

                                case UnitEventTiming.Move: gnDeltaCellPath = 0; loBoardUnit.UnitInstance.OnUnitAnimatorMove = OnUnitAnimatorMoveOnCells; lcAnimationState = nameof(AnimationControllerData.Triggers.Move); break;
                                case UnitEventTiming.CollideEmitter:
                                    loBoardUnit.UnitInstance.OnUnitAnimatorMove = OnUnitAnimatorMoveManually;
                                    if (loBoardUnit.Facing.FromFront(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.KnockBack);
                                    else if (loBoardUnit.Facing.FromBack(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.KnockFront);
                                    else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.KnockLeft);
                                    else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.KnockRight);
                                    else lcAnimationState = nameof(AnimationControllerData.Triggers.KnockBack);
                                    break;
                                case UnitEventTiming.Attack: lcAnimationState = nameof(AnimationControllerData.Triggers.Attack); break;
                                case UnitEventTiming.ReceiveDamage:
                                case UnitEventTiming.CollideReceiver:
                                    if (loGameplayEffectCaller == null || loGameplayEffectCaller.Type != GameplayEffectType.UnitHealthNoAnimation)
                                    {
                                        //<< Revisamos si nos estamos moviendo, de ser así aplicamos mascara, de lo contrario aplicamos la animacion tal cual.
                                        if (loBoardUnit.UnitInstance.OnUnitAnimatorMove != null)
                                        {
                                            //<< Iniciamos animación de recepcion de daño con base en nuestra dirección y la dirección del ataque.
                                            if (loBoardUnit.Facing.FromBack(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageMaskBack);
                                            else if (loBoardUnit.Facing.FromFront(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageMaskFront);
                                            else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageMaskLeft);
                                            else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageMaskRight);
                                            else lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageMaskFront);
                                        }
                                        else
                                        {
                                            //<< Iniciamos animación de recepcion de daño con base en nuestra dirección y la dirección del ataque.
                                            if (loBoardUnit.Facing.FromBack(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageBack);
                                            else if (loBoardUnit.Facing.FromFront(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageFront);
                                            else if (loBoardUnit.Facing.FromLeft(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageLeft);
                                            else if (loBoardUnit.Facing.FromRight(loFacingTarget)) lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageRight);
                                            else lcAnimationState = nameof(AnimationControllerData.Triggers.ReceiveDamageFront);
                                        }
                                    }
                                    break;
                                case UnitEventTiming.Death: lcAnimationState = nameof(AnimationControllerData.Triggers.Death); break;
                                case UnitEventTiming.Creation: lcAnimationState = nameof(AnimationControllerData.Triggers.Creation); break;
                                default: break;
                            }
                        }

                        //<< Si nos indican que se mueve usando la animación activamos el movimiento.
                        if (loInstruction.Command == ClientCommand.ForceAnimationWithRootMotion) loBoardUnit.UnitInstance.OnUnitAnimatorMove = OnUnitAnimatorMove;

                        if (lnAnimationTrigger != 0) loBoardUnit.UnitInstance.animator.Play(lcAnimationState, 0);
                    }
                    break;

                case ClientCommand.StopAnimation: loBoardUnit.UnitInstance.animator.SetTrigger(AnimationControllerData.Triggers.StopAction); break;

                //case ClientCommand.PlayFacingDirectionAnimation:
                //    break;

                case ClientCommand.PlayFacingAnimation:
                    {
                        List<BoardUnit> loUnitsFacing = new List<BoardUnit>();
                        if (loInstruction.OnTarget)
                        {
                            if (loGameplayEffectAreaCells != null)
                            {
                                int lnUnits;
                                BoardUnit loItem;
                                BoardCell<BoardUnit> loCell;
                                for (int c = 0; c < lnGameplayEffectAreaCells; c++)
                                {
                                    loCell = loGameplayEffectAreaCells[c];

                                    lnUnits = loCell.Data.Count;
                                    for (int j = lnUnits - 1; j >= 0; j--)
                                    {
                                        loItem = loCell.Data[j];

                                        //<< Si no es objetivo continuamos.
                                        if (!IsTarget(loItem, loBoardUnit, loGameplayEffect.Target)) continue;

                                        //<< Si somos quien llamó
                                        if (loItem == loBoardUnit) continue;

                                        loUnitsFacing.Add(loItem);
                                    }
                                }
                            }
                        }
                        else loUnitsFacing.Add(loBoardUnit);

                        //<< Aplicamos animacion de rotación en las unidades asignadas.
                        foreach (BoardUnit loItem in loUnitsFacing)
                        {
                            Vector3 loUnitDirection = loItem.UnitInstance.transform.forward;
                            Vector3 loFacingDirection = Vector3.zero;
                            switch (loItem.Facing)
                            {
                                case Facing.ZPos: loFacingDirection = Vector3.forward; break;
                                case Facing.ZNeg: loFacingDirection = Vector3.back; break;
                                case Facing.XPos: loFacingDirection = Vector3.right; break;
                                case Facing.XNeg: loFacingDirection = Vector3.left; break;
                                default: break;
                            }

                            float lnAngle = Vector3.SignedAngle(loUnitDirection, loFacingDirection, Vector3.up);
                            if (Mathf.Abs(lnAngle) < 14.0f) lnAnimationTrigger = 0;
                            else if (Mathf.Abs(lnAngle) > 135.0f) lnAnimationTrigger = AnimationControllerData.Triggers.Turn180;
                            else if (lnAngle > 45.0f) lnAnimationTrigger = AnimationControllerData.Triggers.Turn90Pos;
                            else if (lnAngle < 45.0f) lnAnimationTrigger = AnimationControllerData.Triggers.Turn90Neg;
                            else lnAnimationTrigger = 0;

                            //<< Reproducimos la animación de rotación.
                            if (lnAnimationTrigger != 0 && loItem.UnitInstance.animator != null) loItem.UnitInstance.animator.SetTrigger(lnAnimationTrigger);
                        }
                    }
                    break;
                case ClientCommand.RotateUnitToMatchFacing: loBoardUnit.UnitInstance.transform.rotation = loBoardUnit.Facing.ToQuaternion(); break;

                case ClientCommand.PlayAnimationOnGameplayEffectAreaUnits:
                    {
                        int lnAnimationTriggerIn = Animator.StringToHash(loInstruction.Value);
                        for (int j = 0; j < lnGameplayEffectAreaCells; j++)
                        {
                            foreach (BoardUnit loItem in loGameplayEffectAreaCells[j].Data)
                            {
                                loItem.UnitInstance.animator.SetTrigger(lnAnimationTriggerIn);
                            }
                        }
                    }
                    break;
                case ClientCommand.StopAnimationOnGameplayEffectAreaUnits:
                    {
                        for (int j = 0; j < lnGameplayEffectAreaCells; j++)
                        {
                            foreach (BoardUnit loItem in loGameplayEffectAreaCells[j].Data)
                            {
                                loItem.UnitInstance.animator.SetTrigger(AnimationControllerData.Triggers.StopAction);
                            }
                        }
                    }
                    break;

                case ClientCommand.PlayAnimationOnTargetByAreaSection:
                    {
                        int lnAnimationTriggerIn = Animator.StringToHash(loInstruction.Value);
                        int lnSectionCells = 0;

                        //<< Obtenemos la sección a la cual aplicaremos el efecto.
                        BoardCell<BoardUnit>.GetSectionCells(lnGameplayEffectAreaSection, lnGameplayEffectAreaCells, loGameplayEffectAreaCells, loGameplayEffectAreaSectionCells, out lnSectionCells);

                        //<< Si no hay celdas del area seleccionada continuamos.
                        for (int j = 0; j < lnSectionCells; j++)
                        {
                            foreach (BoardUnit loItem in loGameplayEffectAreaSectionCells[j].Data)
                            {
                                loItem.UnitInstance.animator.SetTrigger(lnAnimationTriggerIn);
                            }
                        }
                    }
                    break;

                case ClientCommand.RequestGameplayEffectAreaCells:
                    {
                        lnGameplayEffectAreaCells = 0;
                        lnGameplayEffectAreaSection = 0;

                        if (loInstruction.GameplayEffectId != null)
                        {
                            //<< Obtenemos el efecto que aplicaremos.
                            loGameplayEffect = goGameplayEffects[loInstruction.GameplayEffectId.Value];

                            //<< Obtenemos el area del efecto.
                            loGameplayEffectAreaCells = GetGameplayEffectAreaCells(loBoardUnit, loGameplayEffect, lnCellIndexTarget, loFacingTarget, out lnGameplayEffectAreaCells);

                            if (loGameplayEffectAreaCells == null) continue;

                            //<< Creamos un shallow clon.
                            BoardCell<BoardUnit>[] loGameplayEffectAreaCellsIn = new BoardCell<BoardUnit>[lnGameplayEffectAreaCells];
                            for (int j = 0; j < lnGameplayEffectAreaCells; j++) loGameplayEffectAreaCellsIn[j] = loGameplayEffectAreaCells[j];
                            loGameplayEffectAreaCells = loGameplayEffectAreaCellsIn;

                            //<< Hacemos espacio para las secciones de celdas.
                            loGameplayEffectAreaSectionCells = new BoardCell<BoardUnit>[lnGameplayEffectAreaCells];

                            //<< Determinamos nuestra area maxima.
                            lnGameplayEffectAreaSections = BoardCell<BoardUnit>.GetMaxDistance(lnGameplayEffectAreaCells, loGameplayEffectAreaCells) + 1;
                        }
                    }
                    break;

                case ClientCommand.RequestGameplayEffectAreaCellsNoFilter:
                    {
                        lnGameplayEffectAreaCells = 0;
                        lnGameplayEffectAreaSection = 0;

                        if (loInstruction.GameplayEffectId != null)
                        {
                            //<< Obtenemos el efecto que aplicaremos.
                            loGameplayEffect = goGameplayEffects[loInstruction.GameplayEffectId.Value];

                            //<< Obtenemos el area del efecto.
                            loGameplayEffectAreaCells = GetGameplayEffectAreaCells(loBoardUnit, loGameplayEffect, lnCellIndexTarget, loFacingTarget, out lnGameplayEffectAreaCells, false);

                            if (loGameplayEffectAreaCells == null) continue;

                            //<< Creamos un shallow clon.
                            BoardCell<BoardUnit>[] loGameplayEffectAreaCellsIn = new BoardCell<BoardUnit>[lnGameplayEffectAreaCells];
                            for (int j = 0; j < lnGameplayEffectAreaCells; j++) loGameplayEffectAreaCellsIn[j] = loGameplayEffectAreaCells[j];
                            loGameplayEffectAreaCells = loGameplayEffectAreaCellsIn;

                            //<< Hacemos espacio para las secciones de celdas.
                            loGameplayEffectAreaSectionCells = new BoardCell<BoardUnit>[lnGameplayEffectAreaCells];

                            //<< Determinamos nuestra area maxima.
                            lnGameplayEffectAreaSections = BoardCell<BoardUnit>.GetMaxDistance(lnGameplayEffectAreaCells, loGameplayEffectAreaCells) + 1;
                        }
                    }
                    break;

                case ClientCommand.MaskHidenUnitsFromGameplayEffectAreaCells:
                    {
                        if (loGameplayEffectAreaCells == null) continue;

                        List<BoardCell<BoardUnit>> loGameplayEffectAreaCellsIn = new List<BoardCell<BoardUnit>>();
                        for (int s = 0; s < lnGameplayEffectAreaCells; s++)
                        {
                            if (loGameplayEffectAreaCells[s].Data.Any(x => x.GameplayEffects.Any(y => y.Type == GameplayEffectType.UnitHide))) continue;
                            loGameplayEffectAreaCellsIn.Add(loGameplayEffectAreaCells[s]);
                        }

                        //<< Pasamos las celdas donde no hay unidades ocultas.
                        loGameplayEffectAreaCells = loGameplayEffectAreaCellsIn.ToArray();
                        lnGameplayEffectAreaCells = loGameplayEffectAreaCellsIn.Count;

                        //<< Hacemos espacio para las secciones de celdas.
                        loGameplayEffectAreaSectionCells = new BoardCell<BoardUnit>[lnGameplayEffectAreaCells];

                        //<< Determinamos nuestra area maxima.
                        lnGameplayEffectAreaSections = BoardCell<BoardUnit>.GetMaxDistance(lnGameplayEffectAreaCells, loGameplayEffectAreaCells) + 1;
                    }
                    break;

                case ClientCommand.AddCallerUnitToGameplayEffectAreaCells:
                    {
                        if (loGameplayEffectAreaCells == null) continue;
                        List<BoardCell<BoardUnit>> loGameplayEffectAreaCellsIn = new List<BoardCell<BoardUnit>>(loGameplayEffectAreaCells);

                        int lnCellIndexRefresh = goBoard.CellIndexOf(loBoardUnit.X, loBoardUnit.Z);
                        BoardCell<BoardUnit> loCell = goBoard.GetCell(lnCellIndexRefresh);

                        //<< Agregamos la celda del caller.
                        loGameplayEffectAreaCellsIn.Insert(0, loCell);

                        //<< Pasamos las celdas donde ya incluimos al caller.
                        loGameplayEffectAreaCells = loGameplayEffectAreaCellsIn.ToArray();
                        lnGameplayEffectAreaCells = loGameplayEffectAreaCellsIn.Count;
                    }
                    break;

                case ClientCommand.RefreshUnitPosition:
                    {
                        int lnCellIndexRefresh = goBoard.CellIndexOf(loBoardUnit.X, loBoardUnit.Z);
                        BoardCell<BoardUnit> loCell = goBoard.GetCell(lnCellIndexRefresh);
                        loBoardUnit.UnitInstance.transform.position = loCell.Vector;
                    }
                    break;

                case ClientCommand.DeactivateUnitEvent: loBoardUnit.UnitInstance.OnDeactivation.Invoke(); break;
                case ClientCommand.SetUnitVisibility:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value) || !bool.TryParse(loInstruction.Value, out bool lbResult)) loBoardUnit.UnitInstance.gameObject.SetActive(false);
                        else loBoardUnit.UnitInstance.gameObject.SetActive(lbResult);
                    }
                    break;

                case ClientCommand.SetUnitRenderersVisibility:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value) || !bool.TryParse(loInstruction.Value, out bool lbResult)) lbResult = false;
                        loBoardUnit.UnitInstance.renderers.SetActive(lbResult);
                    }
                    break;

                case ClientCommand.ClearOnUnitAnimatorMove: loBoardUnit.UnitInstance.OnUnitAnimatorMove = null; break;
                case ClientCommand.ContinueAsynchronous:
                    {
                        //<< Revisamos si mandan cuantas instrucciones debemos seguir de manera asincrona a partir de la 'i'.
                        int lnCount = lnInstructionsCount;
                        int lnOffset = 0;
                        if (!string.IsNullOrWhiteSpace(loInstruction.Value) && int.TryParse(loInstruction.Value, out lnOffset)) lnCount = i + 1 + lnOffset;

                        StartCoroutine(PlayEvent(loUnitEventTiming, loBoardUnitCaller, loBoardUnit, lnCellIndexTarget, loFacingTarget, loInstructions, lnCount, i + 1));

                        if (lnOffset > 0) i += lnOffset;
                        else yield break;
                    }
                    break;

                case ClientCommand.WaitForEndOfFrame: yield return new WaitForEndOfFrame(); break;
                case ClientCommand.WaitForSeconds: if (float.TryParse(loInstruction.Value, out float lnSeconds)) yield return new WaitForSeconds(lnSeconds); break;
                case ClientCommand.WaitForAnimationFlag:
                    lnAnimationFlag++;
                    while (loBoardUnit.UnitInstance.GetAnimationFlag() < lnAnimationFlag) yield return loWaitForEndOfFrame;
                    loBoardUnit.UnitInstance.DeactivateAnimationFlag();
                    lnAnimationFlag--;
                    break;
                case ClientCommand.WaitForAnimationEnd:
                    while (lnAnimationTrigger != 0)
                    {
                        yield return loWaitForEndOfFrame;
                        if (loBoardUnit.UnitInstance == null) yield break;
                        if (loBoardUnit.UnitInstance.animator.IsAnimationActive(lnAnimationTrigger)) continue;
                        break;
                    }
                    break;
                case ClientCommand.WaitForMoveEnd:
                    {
                        float lnNextCheck = 0.0f;
                        int lnNextCell = 0;

                        BoardCell<BoardUnit> loBoardCellPrev = goUnitCellsSelected[0];
                        BoardCell<BoardUnit> loBoardCellNext = goUnitCellsSelected[0];
                        BoardUnit loBoardUnitCell;
                        int lnCountIn;

                        //<< Esperamos a que la unidad termine de moverse para continuar.                        
                        while (loBoardUnit.UnitInstance.OnUnitAnimatorMove != null)
                        {
                            //<< Indicamos el elemento de la proxima revisión.
                            lnNextCheck = lnNextCell + 0.5f;

                            //<< Esperamos a llegar a la mitad de la primer celda para llamar el evento.
                            while (loBoardUnit.UnitInstance.OnUnitAnimatorMove != null && gnDeltaCellPath < lnNextCheck) yield return loWaitForEndOfFrame;

                            //<< Revisamos si es la mitad de la primer celda, de ser así aplicamos el evento.
                            if (lnNextCheck == 0.5f) yield return ApplyEvent(UnitEventTiming.FirstHalfCell, loBoardUnit, loBoardUnit, lnCellIndexTarget, loFacingTarget);

                            //<< Revisamos si hay alguna unidad en nuestra celda siguiente.
                            lnNextCell = Mathf.CeilToInt(gnDeltaCellPath);
                            if (lnNextCell > -1 && lnNextCell < goUnitCellsSelected.Count)
                            {
                                loBoardCellPrev = loBoardCellNext;
                                loBoardCellNext = goUnitCellsSelected[lnNextCell];

                                //<< Solo si hay unidades en la celda.
                                if (loBoardCellNext.Data.Count > 0)
                                {
                                    //<< Como es una unidad que no sea la misma aplicamos colisión.
                                    if (!loBoardCellNext.Data.Contains(loBoardUnit) && loBoardCellNext.Data.Any(x => x.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)))
                                    {
                                        //<< Detenemos el movimiento de la unidad.
                                        EndUnitInstanceMove(loBoardUnit.UnitInstance);

                                        loBoardUnitCell = loBoardCellNext.Data.FirstOrDefault(x => x.Selectable());

                                        //<< Ya que habrá colisión el ultimo facing será en dirección en la que queda la unidad en ves de rotar al frente.
                                        loBoardUnit.Facing = FacingExtensions.GetFacing(loBoardCellPrev.X, loBoardCellPrev.Z, loBoardCellNext.X, loBoardCellNext.Z, loBoardUnit.Facing);

                                        //<< Aplicamos el evento de colisión del receptor que regularmente no bloquea.
                                        loBoardUnitCell.TargetX = loBoardUnit.X;
                                        loBoardUnitCell.TargetZ = loBoardUnit.Z;
                                        yield return ApplyEvent(UnitEventTiming.CollideReceiver, loBoardUnit, loBoardUnitCell, loBoardCellNext.Index, loBoardUnitCell.Facing);

                                        //<< Aplicamos el evento de colisión del emisor que si bloquea.
                                        yield return ApplyEvent(UnitEventTiming.CollideEmitter, loBoardUnit, loBoardUnit, lnCellIndexTarget, loBoardUnit.Facing);
                                    }
                                    //<< Si no es de tipo unidad debemos movernos el resto del camino y aplicar lo que haya.
                                    else
                                    {
                                        //<< Aplicamos el evento de "StepOut" que regularmente no bloquea. Nota: Indicamos que no deben aplicar facingRule ya que nos estamos moviendo.
                                        //yield return ApplyEvent(UnitEventTiming.StepOut, loUnitStoredPropertiesCaller, loUnitStoredPropertiesCaller, lnCellIndex, loFacing);
                                        lnCountIn = loBoardCellPrev.Data.Count;
                                        for (int c = lnCountIn - 1; c >= 0; c--)
                                        {
                                            loBoardUnitCell = loBoardCellPrev.Data[c];
                                            yield return ApplyEvent(UnitEventTiming.StepOut, loBoardUnit, loBoardUnitCell, loBoardCellNext.Index, loFacingTarget);
                                        }

                                        //<< Aplicamos el evento de "StepIn" que regularmente no bloquea. Nota: Indicamos que no deben aplicar facingRule ya que nos estamos moviendo.
                                        lnCountIn = loBoardCellNext.Data.Count;
                                        for (int c = lnCountIn - 1; c >= 0; c--)
                                        {
                                            loBoardUnitCell = loBoardCellNext.Data[c];
                                            yield return ApplyEvent(UnitEventTiming.StepIn, loBoardUnit, loBoardUnitCell, loBoardCellNext.Index, loFacingTarget);
                                        }
                                    }
                                }

                                //<< Esperamos a llegar a la celda para llamar el evento de InsideCell.
                                while (loBoardUnit.UnitInstance.OnUnitAnimatorMove != null && gnDeltaCellPath < lnNextCell) yield return loWaitForEndOfFrame;

                                //<< Si no estamos en la celda significa que estamos de paso pero llamamos en la unidad que está de paso.
                                if (!loBoardCellNext.Data.Contains(loBoardUnit)) yield return ApplyEvent(UnitEventTiming.InsideCell, loBoardUnit, loBoardUnit, loBoardCellNext.Index, loFacingTarget);

                                //<< Avisamos a los elementos de la celda que hay una unidad dentro.
                                lnCountIn = loBoardCellNext.Data.Count;
                                for (int c = lnCountIn - 1; c >= 0; c--)
                                {
                                    loBoardUnitCell = loBoardCellNext.Data[c];
                                    yield return ApplyEvent(UnitEventTiming.InsideCell, loBoardUnit, loBoardUnitCell, loBoardCellNext.Index, loFacingTarget);
                                }
                            }

                            yield return loWaitForEndOfFrame;
                        }

                        loBoardCellNext = goBoard.GetCell(lnCellIndexTarget);
                        lnCountIn = loBoardCellNext.Data.Count;
                        for (int c = lnCountIn - 1; c >= 0; c--)
                        {
                            loBoardUnitCell = loBoardCellNext.Data[c];
                            yield return ApplyEvent(UnitEventTiming.Arrival, loBoardUnit, loBoardUnitCell, lnCellIndexTarget, loFacingTarget);
                        }
                    }
                    break;

                case ClientCommand.WaitForPushEnd:
                    {
                        //<< Para cada celda obtenida...
                        int lnPushCells = gnPushCells;
                        BoardCell<BoardUnit> loCellOrigin = goBoard.GetCell(lnCellIndexTarget);
                        BoardCell<BoardUnit> loCell = loCellOrigin;
                        BoardCell<BoardUnit> loCollisionedUnitCell;
                        switch (loFacingTarget)
                        {
                            case Facing.ZPos: loCollisionedUnitCell = loCell.GetLinearCellZPos(PushThroughCell, ref lnPushCells); break;
                            case Facing.ZNeg: loCollisionedUnitCell = loCell.GetLinearCellZNeg(PushThroughCell, ref lnPushCells); break;
                            case Facing.XPos: loCollisionedUnitCell = loCell.GetLinearCellXPos(PushThroughCell, ref lnPushCells); break;
                            case Facing.XNeg: loCollisionedUnitCell = loCell.GetLinearCellXNeg(PushThroughCell, ref lnPushCells); break;
                            default: loCollisionedUnitCell = null; break;
                        }

                        int lnCells = 0;
                        float lnDistance = 0.0f;
                        int lnCountIn;
                        int lnCellIndexIn;

                        float lnStep;
                        Vector3 loUnitPosition = loBoardUnit.UnitInstance.transform.position;

                        //<< Realizamos la traslacion de la unidad entre las celdas.
                        while (lnCells < lnPushCells)
                        {
                            lnCells++;

                            //<< Movemos a la unidad con base en la dirección dada.
                            //<< Esperamos a llegar a la siguiente distancia de la celda para llamar el evento correspondiente.
                            switch (loFacingTarget)
                            {
                                case Facing.ZNeg:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.z -= lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                                case Facing.XPos:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.x += lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                                case Facing.XNeg:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.x -= lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;

                                case Facing.ZPos:
                                default:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.z += lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                            }

                            //<< Indicamos el indice de la celda.
                            lnCellIndexIn = loCell.Index;

                            //<< Aplicamos el evento de "StepOut" que regularmente no bloquea. Nota: Indicamos que no deben aplicar facingRule ya que nos estamos moviendo.
                            yield return ApplyEvent(UnitEventTiming.StepOut, loBoardUnit, loBoardUnit, lnCellIndexIn, loFacingTarget);

                            //<< Asignamos la nueva posicion de la unidad.
                            switch (loFacingTarget)
                            {
                                case Facing.ZPos:
                                    ChangeUnitCell(loBoardUnit, loCell, loCell.ZPos);

                                    //<< Sabemos que pasamos a la siguiente celda por ello aplicamos el efecto que tenga.
                                    loCell = loCell.ZPos;
                                    break;

                                case Facing.ZNeg:
                                    ChangeUnitCell(loBoardUnit, loCell, loCell.ZNeg);

                                    //<< Sabemos que pasamos a la siguiente celda por ello aplicamos el efecto que tenga.
                                    loCell = loCell.ZNeg;
                                    break;
                                case Facing.XPos:
                                    ChangeUnitCell(loBoardUnit, loCell, loCell.XPos);

                                    //<< Sabemos que pasamos a la siguiente celda por ello aplicamos el efecto que tenga.
                                    loCell = loCell.XPos;
                                    break;
                                case Facing.XNeg:
                                    ChangeUnitCell(loBoardUnit, loCell, loCell.XNeg);

                                    //<< Sabemos que pasamos a la siguiente celda por ello aplicamos el efecto que tenga.
                                    loCell = loCell.XNeg;
                                    break;

                                default: break;
                            }

                            //<< Indicamos el indice de la celda.
                            lnCellIndexIn = loCell.Index;

                            //<< Aplicamos el evento de "StepIn" que regularmente no bloquea.
                            lnCountIn = loCell.Data.Count;
                            for (int c = lnCountIn - 1; c >= 0; c--) yield return ApplyEvent(UnitEventTiming.StepIn, loBoardUnit, loCell.Data[c], lnCellIndexIn, loFacingTarget);

                            //<< Movemos a la unidad con base en la dirección dada. (Por segunda ocasión ya que las celdas son de 1m)
                            //<< Esperamos a llegar a la siguiente distancia de la celda para llamar el evento correspondiente.
                            switch (loFacingTarget)
                            {
                                case Facing.ZNeg:
                                    while (lnDistance < 1.0f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.z -= lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                                case Facing.XPos:
                                    while (lnDistance < 1.0f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.x += lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                                case Facing.XNeg:
                                    while (lnDistance < 1.0f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.x -= lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;

                                case Facing.ZPos:
                                default:
                                    while (lnDistance < 1.0f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.z += lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                            }

                            //<< Reducimos en la distancia que recorrimos.
                            lnDistance -= 1.0f;
                        }

                        //<< Revisamos si logramos mover la unidad todas las celdas.
                        if (loCollisionedUnitCell != null && !loCollisionedUnitCell.Data.Contains(loBoardUnit) && loCollisionedUnitCell.Data.Any(x => x.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)))
                        {
                            //<< Esperamos llegar a media celda para aplicar la colisión.
                            switch (loFacingTarget)
                            {
                                case Facing.ZNeg:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.z -= lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                                case Facing.XPos:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.x += lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                                case Facing.XNeg:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.x -= lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;

                                case Facing.ZPos:
                                default:
                                    while (lnDistance < 0.5f)
                                    {
                                        lnStep = Time.deltaTime * gnPushSpeed;
                                        lnDistance += lnStep;
                                        loUnitPosition.z += lnStep;
                                        loBoardUnit.UnitInstance.transform.position = loUnitPosition;
                                        yield return loWaitForEndOfFrame;
                                    }
                                    break;
                            }

                            BoardUnit loBoardUnitCell = loCollisionedUnitCell.Data.FirstOrDefault(x => x.Selectable());

                            //<< Aplicamos el daño de la colisión del push "PushCollision" que regularmente no bloquea.
                            yield return ApplyEvent(UnitEventTiming.PushCollision, loBoardUnit, loBoardUnitCell, loCollisionedUnitCell.Index, loBoardUnitCell.Facing);

                            //<< Aplicamos el evento de colisión del receptor que regularmente no bloquea.
                            loBoardUnitCell.TargetX = loBoardUnit.X;
                            loBoardUnitCell.TargetZ = loBoardUnit.Z;
                            yield return ApplyEvent(UnitEventTiming.CollideReceiver, loBoardUnit, loBoardUnitCell, loCollisionedUnitCell.Index, loBoardUnitCell.Facing);

                            //<< Aplicamos el evento de colisión del emisor que si bloquea.
                            yield return ApplyEvent(UnitEventTiming.CollideEmitter, loBoardUnit, loBoardUnit, lnCellIndexTarget, loBoardUnit.Facing);
                        }
                        else
                        {
                            //<< Detenemos la acción del push.
                            loBoardUnit.UnitInstance.animator.SetTrigger(AnimationControllerData.Triggers.StopAction);
                        }

                        //<< Ya que llegamos al destino llamamos el evento de llegada.
                        lnCountIn = loCell.Data.Count;
                        for (int c = lnCountIn - 1; c >= 0; c--) yield return ApplyEvent(UnitEventTiming.Arrival, loBoardUnit, loCell.Data[c], loCell.Index, loFacingTarget);

                        //<< Incrementamos el callback ya que sabemos que cuandos e hace Push se espera que se acomplete la cuenta para continuar.
                        gnCallbackCount++;
                    }
                    break;

                case ClientCommand.WaitForPlayablePrefabEnd:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Revisamos si ha terminado, de no ser así esperamos.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        while (!loPlayablePrefab.Ended()) yield return loWaitForEndOfFrame;
                    }
                    break;

                case ClientCommand.WaitForPlayablePrefabDeactivation:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Revisamos si ha sido desactivado, de no ser así esperamos.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        while (loPlayablePrefab.gameObject.activeSelf) yield return loWaitForEndOfFrame;
                    }
                    break;

                case ClientCommand.ScaleUnitInstance:
                    {
                        if (float.TryParse(loInstruction.Value, out float lnScale))
                        {
                            Vector3 loScale = new Vector3(lnScale, lnScale, lnScale);
                            if (loInstruction.OnTarget)
                            {
                                if (loGameplayEffectAreaCells != null)
                                {
                                    int lnUnits;
                                    BoardUnit loItem;
                                    BoardCell<BoardUnit> loCell;
                                    for (int c = 0; c < lnGameplayEffectAreaCells; c++)
                                    {
                                        loCell = loGameplayEffectAreaCells[c];

                                        lnUnits = loCell.Data.Count;
                                        for (int j = lnUnits - 1; j >= 0; j--)
                                        {
                                            loItem = loCell.Data[j];

                                            //<< Si no es objetivo continuamos.
                                            if (!IsTarget(loItem, loBoardUnit, loGameplayEffect.Target)) continue;

                                            //<< Si somos quien llamó
                                            if (loItem == loBoardUnit) continue;

                                            loItem.UnitInstance.transform.localScale = loScale;
                                        }
                                    }
                                }
                            }
                            else loBoardUnit.UnitInstance.transform.localScale = loScale;
                        }
                    }
                    break;

                case ClientCommand.ScaleUnitInstanceFirstChild:
                    {
                        if (float.TryParse(loInstruction.Value, out float lnScale))
                        {
                            Vector3 loScale = new Vector3(lnScale, lnScale, lnScale);
                            if (loInstruction.OnTarget)
                            {
                                if (loGameplayEffectAreaCells != null)
                                {
                                    int lnUnits;
                                    BoardUnit loItem;
                                    BoardCell<BoardUnit> loCell;
                                    for (int c = 0; c < lnGameplayEffectAreaCells; c++)
                                    {
                                        loCell = loGameplayEffectAreaCells[c];

                                        lnUnits = loCell.Data.Count;
                                        for (int j = lnUnits - 1; j >= 0; j--)
                                        {
                                            loItem = loCell.Data[j];

                                            //<< Si no es objetivo continuamos.
                                            if (!IsTarget(loItem, loBoardUnit, loGameplayEffect.Target)) continue;

                                            //<< Si somos quien llamó
                                            if (loItem == loBoardUnit) continue;

                                            //<< Si no hay hijos del transform.
                                            if (loBoardUnit.UnitInstance.transform.childCount <= 0) continue;

                                            loItem.UnitInstance.transform.GetChild(0).localScale = loScale;
                                        }
                                    }
                                }
                            }
                            else if (loBoardUnit.UnitInstance.transform.childCount > 0) loBoardUnit.UnitInstance.transform.GetChild(0).localScale = loScale;
                        }
                    }
                    break;

                case ClientCommand.CreatePlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si ya existe en la lista continuamos.
                        if (loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Creamos el efecto con los elementos visuales.
                        PlayablePrefab loPlayablePrefab = CreatePlayblePrefab(loInstruction.VisualEffectAssetBundleId.Value);

                        //<< Si no se pudo crear continuamos.
                        if (loPlayablePrefab == null) continue;

                        //<< Le pasamos el valor si existe.
                        if (!string.IsNullOrWhiteSpace(loInstruction.Value)) loPlayablePrefab.SetValue(loInstruction.Value);

                        //<< Asignamos posicion y rotación.
                        if (loUnitEventTiming != UnitEventTiming.None)
                        {
                            loPlayablePrefab.transform.position = loBoardUnit.UnitInstance.transform.position;
                            loPlayablePrefab.transform.rotation = loBoardUnit.Facing.ToQuaternion();
                        }
                        else
                        {
                            loPlayablePrefab.transform.position = goBoard.GetCell(lnCellIndexTarget).Vector;
                            loPlayablePrefab.transform.rotation = loFacingTarget.ToQuaternion();
                        }

                        //<< Almacenamos para detener mas adelante.
                        loPlayablePrefabs.Add(loInstruction.VisualEffectAssetBundleId.Value, loPlayablePrefab);
                    }
                    break;
                case ClientCommand.PlayPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Reproducimos.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.gameObject.SetActive(true);

                        //<< Le pasamos el valor si existe.
                        if (!string.IsNullOrWhiteSpace(loInstruction.Value)) loPlayablePrefab.SetValue(loInstruction.Value);

                        //<< Reproducimos coroutine de los targets de ser previamente asignados.
                        if (loGameplayEffectAreaCells != null) loPlayablePrefab.Play(loInstruction.Value, lnGameplayEffectAreaSections, loGameplayEffectAreaCells);
                        else loPlayablePrefab.Play(loInstruction.Value, 1, goBoard.GetCell(lnCellIndexTarget));
                    }
                    break;
                case ClientCommand.StopPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Detenemos.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].Stop();
                    }
                    break;
                case ClientCommand.RemovePlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Removemos.
                        loPlayablePrefabs.Remove(loInstruction.VisualEffectAssetBundleId.Value);
                    }
                    break;
                case ClientCommand.PreviewPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Preveemos.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].Preview();
                    }
                    break;

                case ClientCommand.ConvertStoredPositionScreenToWorld:
                    {
                        //<< Obtenemos la posición del UI en pantalla.
                        Vector2 loTargetScreenPos = RectTransformUtility.WorldToScreenPoint(null, goStoredPosition);

                        //<< Convertimos la posición a mundo.
                        goStoredPosition = Camera.main.ScreenToWorldPoint(new Vector3(loTargetScreenPos.x, loTargetScreenPos.y, 0.0f));
                    }
                    break;

                case ClientCommand.AnimatePlayablePrefabTranslationToStoredPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Obtenemos el playable prefab para animarlo a la posición requerida.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        float lnTime = 1.0f;
                        //<< Obtenemos el valor de tiempo si existe.
                        float.TryParse(loInstruction.Value, out lnTime);
                        Vector3 loPositionStart = loPlayablePrefab.transform.position;

                        float lnDelta;
                        AnimationCurve loAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                        MicroAnimation loMicroAnimation = new MicroAnimation(lnTime);
                        loMicroAnimation.SetAsAAnimateToB();
                        while (loMicroAnimation.IsAnimating())
                        {
                            loMicroAnimation.Update();
                            lnDelta = loAnimationCurve.Evaluate(loMicroAnimation.GetDelta());
                            loPlayablePrefab.transform.position = Vector3.Lerp(loPositionStart, goStoredPosition, lnDelta);
                            yield return loWaitForEndOfFrame;
                        }
                    }
                    break;

                case ClientCommand.AttachPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos padre.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        Transform loAnchor;
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) loAnchor = loBoardUnit.UnitInstance.transform;
                        else loAnchor = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);

                        loPlayablePrefab.transform.SetParent(loAnchor, false);
                        loPlayablePrefab.transform.localPosition = Vector3.zero;
                        loPlayablePrefab.transform.rotation = Quaternion.identity;
                        loPlayablePrefab.transform.localScale = Vector3.one;
                    }
                    break;
                case ClientCommand.DetachPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Removemos padre.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.SetParent(null, true);
                    }
                    break;

                case ClientCommand.DeactivateAnyChildInTransform:
                    {
                        Transform loParent;
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;
                        loParent = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);
                        if (loParent == null) continue;
                        int lnChildCount = loParent.childCount;
                        for (int c = 0; c < lnChildCount; c++) loParent.GetChild(c).gameObject.SetActive(false);
                    }
                    break;

                case ClientCommand.ParentPlayblePrefabToUI:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Avisamos que debemos asignar el transform (hijo) al UI.
                        Send(GameCommand.UICombatSetUIEffectChild, loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform);
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToInvokerPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        if (!goUnitsGameplayBattleDummy.ContainsKey(loBoardUnit.UnitProperties.Id)) continue;
                        UnitGameplayBattle loUnitGameplayBattle = goUnitsGameplayBattleDummy[loBoardUnit.UnitProperties.Id];

                        UnitStoredProperties loInvoker = GetUnitStoredProperties(loUnitGameplayBattle.InvokedByUnitStoredPropertiesId.Value);

                        //<< Si no se encontró no aplicamos.
                        if (loInvoker == null) continue;

                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.position = loInvoker.GetPosition();
                    }
                    break;

                case ClientCommand.ResetLocalTransformPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos rotacion.
                        PlayablePrefab loPlayablePrefabIn = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefabIn.transform.localPosition = Vector3.zero;
                        loPlayablePrefabIn.transform.localRotation = Quaternion.identity;
                        loPlayablePrefabIn.transform.localScale = Vector3.one;
                    }
                    break;

                case ClientCommand.RotateXPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos rotacion.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localRotation *= Quaternion.Euler(float.Parse(loInstruction.Value), 0.0f, 0.0f);
                    }
                    break;
                case ClientCommand.RotateYPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos rotacion.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localRotation *= Quaternion.Euler(0.0f, float.Parse(loInstruction.Value), 0.0f);
                    }
                    break;
                case ClientCommand.RotateZPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos rotacion.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, float.Parse(loInstruction.Value));
                    }
                    break;
                case ClientCommand.TranslateXPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += new Vector3(float.Parse(loInstruction.Value), 0.0f, 0.0f);
                    }
                    break;
                case ClientCommand.TranslateYPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += new Vector3(0.0f, float.Parse(loInstruction.Value), 0.0f);
                    }
                    break;
                case ClientCommand.TranslateZPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += new Vector3(0.0f, 0.0f, float.Parse(loInstruction.Value));
                    }
                    break;

                case ClientCommand.TranslateXPlayablePrefabUnitBased:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(float.Parse(loInstruction.Value), 0.0f, 0.0f);
                        loTranslation = loBoardUnit.UnitInstance.transform.TransformDirection(loTranslation);
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += loTranslation;
                    }
                    break;

                case ClientCommand.TranslateYPlayablePrefabUnitBased:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, float.Parse(loInstruction.Value), 0.0f);
                        loTranslation = loBoardUnit.UnitInstance.transform.TransformDirection(loTranslation);
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += loTranslation;
                    }
                    break;

                case ClientCommand.TranslateZPlayablePrefabUnitBased:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, 0.0f, float.Parse(loInstruction.Value));
                        loTranslation = loBoardUnit.UnitInstance.transform.TransformDirection(loTranslation);
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += loTranslation;
                    }
                    break;

                case ClientCommand.TranslateXPlayablePrefabCallerUnitBased:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(float.Parse(loInstruction.Value), 0.0f, 0.0f);
                        loTranslation = loBoardUnitCaller.UnitInstance.transform.TransformDirection(loTranslation);
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += loTranslation;
                    }
                    break;

                case ClientCommand.TranslateYPlayablePrefabCallerUnitBased:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, float.Parse(loInstruction.Value), 0.0f);
                        loTranslation = loBoardUnitCaller.UnitInstance.transform.TransformDirection(loTranslation);
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += loTranslation;
                    }
                    break;

                case ClientCommand.TranslateZPlayablePrefabCallerUnitBased:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, 0.0f, float.Parse(loInstruction.Value));
                        loTranslation = loBoardUnitCaller.UnitInstance.transform.TransformDirection(loTranslation);
                        loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value].transform.localPosition += loTranslation;
                    }
                    break;

                case ClientCommand.StoreUITransform:
                    {
                        //<< Pedimos que nos den el transform de la interfaz con el nombre que se pasa en la propiedad de "Value".
                        Send(GameCommand.RequestUITransform, loBoardUnit.Opponent, loInstruction.Value, new UnityAction<Transform>(OnGetRequestedUITransform));
                    }
                    break;

                case ClientCommand.StoreUnitTransform:
                    if (string.IsNullOrWhiteSpace(loInstruction.Value)) goStoredTransform = loBoardUnit.UnitInstance.transform;
                    else goStoredTransform = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);
                    break;

                case ClientCommand.StoreUnitCallerTransform:
                    if (string.IsNullOrWhiteSpace(loInstruction.Value)) goStoredTransform = loBoardUnitCaller.UnitInstance.transform;
                    else goStoredTransform = loBoardUnitCaller.UnitInstance.transform.FindNearest(loInstruction.Value);
                    break;

                case ClientCommand.StorePlayablePrefabTransform:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) goStoredTransform = loPlayablePrefab.transform;
                        else goStoredTransform = loPlayablePrefab.transform.FindNearest(loInstruction.Value);
                    }
                    break;

                case ClientCommand.StoreStoredTransformPosition:
                    if (goStoredTransform != null) goStoredPosition = goStoredTransform.position;
                    break;

                case ClientCommand.StoreUnitPosition:
                    goStoredPosition = loBoardUnit.UnitInstance.transform.position;
                    break;
                case ClientCommand.StoreUniCallertPosition:
                    goStoredPosition = loBoardUnitCaller.UnitInstance.transform.position;
                    break;
                case ClientCommand.TranslateXStoredPositionUnitBased:
                    {
                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(float.Parse(loInstruction.Value), 0.0f, 0.0f);
                        loTranslation = loBoardUnit.UnitInstance.transform.TransformDirection(loTranslation);
                        goStoredPosition += loTranslation;
                    }
                    break;
                case ClientCommand.TranslateYStoredPositionUnitBased:
                    {
                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, float.Parse(loInstruction.Value), 0.0f);
                        loTranslation = loBoardUnit.UnitInstance.transform.TransformDirection(loTranslation);
                        goStoredPosition += loTranslation;
                    }
                    break;
                case ClientCommand.TranslateZStoredPositionUnitBased:
                    {
                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, 0.0f, float.Parse(loInstruction.Value));
                        loTranslation = loBoardUnit.UnitInstance.transform.TransformDirection(loTranslation);
                        goStoredPosition += loTranslation;
                    }
                    break;
                case ClientCommand.TranslateXStoredPositionCallerUnitBased:
                    {
                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(float.Parse(loInstruction.Value), 0.0f, 0.0f);
                        loTranslation = loBoardUnitCaller.UnitInstance.transform.TransformDirection(loTranslation);
                        goStoredPosition += loTranslation;
                    }
                    break;
                case ClientCommand.TranslateYStoredPositionCallerUnitBased:
                    {
                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, float.Parse(loInstruction.Value), 0.0f);
                        loTranslation = loBoardUnitCaller.UnitInstance.transform.TransformDirection(loTranslation);
                        goStoredPosition += loTranslation;
                    }
                    break;
                case ClientCommand.TranslateZStoredPositionCallerUnitBased:
                    {
                        //<< Asignamos translación.
                        Vector3 loTranslation = new Vector3(0.0f, 0.0f, float.Parse(loInstruction.Value));
                        loTranslation = loBoardUnitCaller.UnitInstance.transform.TransformDirection(loTranslation);
                        goStoredPosition += loTranslation;
                    }
                    break;

                case ClientCommand.TranslateXStoredPosition: goStoredPosition.x += float.Parse(loInstruction.Value); break;
                case ClientCommand.TranslateYStoredPosition: goStoredPosition.y += float.Parse(loInstruction.Value); break;
                case ClientCommand.TranslateZStoredPosition: goStoredPosition.z += float.Parse(loInstruction.Value); break;

                case ClientCommand.ScaleXPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos escala.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        Vector3 loScale = loPlayablePrefab.transform.localScale;
                        loScale.x = float.Parse(loInstruction.Value);
                        loPlayablePrefab.transform.localScale = loScale;
                    }
                    break;
                case ClientCommand.ScaleYPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos escala.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        Vector3 loScale = loPlayablePrefab.transform.localScale;
                        loScale.y = float.Parse(loInstruction.Value);
                        loPlayablePrefab.transform.localScale = loScale;
                    }
                    break;
                case ClientCommand.ScaleZPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos escala.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        Vector3 loScale = loPlayablePrefab.transform.localScale;
                        loScale.z = float.Parse(loInstruction.Value);
                        loPlayablePrefab.transform.localScale = loScale;
                    }
                    break;
                case ClientCommand.ScaleXYZPlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos escala.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        Vector3 loScale = loPlayablePrefab.transform.localScale;
                        float lnValue = float.Parse(loInstruction.Value);
                        loScale.Set(lnValue, lnValue, lnValue);
                        loPlayablePrefab.transform.localScale = loScale;
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToTransformPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos padre.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        Transform loBone = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);
                        if (loBone == null) continue;
                        loPlayablePrefab.transform.position = loBone.position;
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToTransformPositionHalfWay:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Asignamos padre.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        Transform loBone = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);
                        if (loBone == null) continue;
                        loPlayablePrefab.transform.position = Vector3.Lerp(loPlayablePrefab.transform.position, loBone.position, 0.5f);
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToUnitRootPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion root de la unidad.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.position = loBoardUnit.UnitInstance.transform.position;
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToCellPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion root de la unidad.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.position = goBoard.GetCell(lnCellIndexTarget).Vector;
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToBoardCenter:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion root de la unidad.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.position = new Vector3(goBoard.Width * 0.5f, 0.0f, goBoard.Lenght * 0.5f);
                    }
                    break;

                case ClientCommand.TranslatePlayblePrefabToZeroLocalPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion root de la unidad.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.localPosition = Vector3.zero;
                    }
                    break;

                case ClientCommand.TranslatePlayblePrefabToTargetPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion de la celda del objetivo.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.position = goBoard.GetCell(lnCellIndexTarget).Vector;
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToUnitForwardPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion de la celda del objetivo.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        float.TryParse(loInstruction.Value, out float lnValue);
                        loPlayablePrefab.transform.position += loBoardUnit.Facing.Forward() * lnValue;
                    }
                    break;

                case ClientCommand.RotatePlayblePrefabToZeroLocalRotation:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion root de la unidad.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.localRotation = Quaternion.identity;
                    }
                    break;

                case ClientCommand.RotatePlayablePrefabToUnitFacing:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Rotamos el playable prefab a la orientacion de la unidad.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.rotation = loBoardUnit.Facing.ToQuaternion();
                    }
                    break;

                case ClientCommand.StoreBonePosition:
                    {
                        Transform loBone = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);
                        if (loBone == null) continue;
                        goStoredPosition = loBone.position;
                    }
                    break;

                case ClientCommand.StoreUIPosition:
                    {
                        //<< Pedimos que nos den la position de la interfaz con el nombre que se pasa en la propiedad de "Value".
                        Send(GameCommand.RequestUIPosition, loBoardUnit.Opponent, loInstruction.Value, new UnityAction<Vector3>(OnGetRequestedUIPosition));
                    }
                    break;

                case ClientCommand.ShowMessageUnitMoved:
                    {
                        //<< FINDME Falta meter mensajes.
                        Debug.Log(GameManager.GetText(GameTextUsage.Sentence, GameTextSentence.RogueMoved));
                    }
                    break;

                case ClientCommand.ShowMessageReceivedItem:
                    {
                        //<< FINDME Falta meter mensajes.
                        Debug.Log(GameManager.GetText(GameTextUsage.Sentence, GameTextSentence.ObjectReceived));
                    }
                    break;

                case ClientCommand.CreateBillboardGameplayItemIcon:
                    {
                        //<< TODO: Por ahora el item es random ya con servidor el indica que id de item es.
                        //if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;
                        //if (!int.TryParse(loInstruction.Value, out int lnGameplayItemId)) continue;

                        if (goGameplayItems.Count == 0) continue;
                        int lnIndex = UnityEngine.Random.Range(0, goGameplayItems.Count);
                        GameplayItem loGameplayItem = goGameplayItems.ElementAt(lnIndex).Value;

                        loBillboardIcon = GameManager.Spawn(prefabBillboardQuad.GetHashCode(), prefabBillboardQuad);
                        loBillboardIcon.material.mainTexture = goGameTextures2D[loGameplayItem.GameTexture2DId].Texture;
                    }
                    break;

                case ClientCommand.SetPlayablePrefabAsBillboardIconParent:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion de la celda del objetivo.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loBillboardIcon.transform.SetParent(loPlayablePrefab.transform);
                        loBillboardIcon.transform.localPosition = Vector3.zero;
                        loBillboardIcon.transform.localScale = Vector3.one;
                        loBillboardIcon.transform.localRotation = Quaternion.identity;
                    }
                    break;

                case ClientCommand.MimeInvokerAnimation:
                    {
                        if (!goUnitsGameplayBattleDummy.ContainsKey(loBoardUnit.UnitStoredPropertiesId)) continue;
                        UnitGameplayBattle loUnitGameplayBattle = goUnitsGameplayBattleDummy[loBoardUnit.UnitStoredPropertiesId];

                        UnitStoredProperties loInvoker = GetUnitStoredProperties(loUnitGameplayBattle.InvokedByUnitStoredPropertiesId.Value);

                        //<< Si no se encontró no aplicamos.
                        if (loInvoker == null) continue;

                        //<< Aplicamos la animación con base en la que lleva el caller.
                        AnimatorStateInfo loAnimatorStateInfo = loInvoker.UnitInstance.animator.GetCurrentAnimatorStateInfo(0);

                        //<< Reproducimos el mismo estado y tiempo que la unidad.
                        loBoardUnit.UnitInstance.animator.Play(loAnimatorStateInfo.fullPathHash, 0, loAnimatorStateInfo.normalizedTime);

                        //<< Aplicamos inmediatamente.
                        loBoardUnit.UnitInstance.animator.Update(0.0f);
                    }
                    break;

                case ClientCommand.RotateInvokerUnitToMatchFacing:
                    {
                        if (!goUnitsGameplayBattleDummy.ContainsKey(loBoardUnit.UnitStoredPropertiesId)) continue;
                        UnitGameplayBattle loUnitGameplayBattle = goUnitsGameplayBattleDummy[loBoardUnit.UnitStoredPropertiesId];

                        UnitStoredProperties loInvoker = GetUnitStoredProperties(loUnitGameplayBattle.InvokedByUnitStoredPropertiesId.Value);

                        //<< Si no se encontró no aplicamos.
                        if (loInvoker == null) continue;

                        loInvoker.UnitInstance.transform.rotation = loInvoker.Facing.ToQuaternion();
                    }
                    break;

                case ClientCommand.RefreshInvokerUnitPosition:
                    {
                        if (!goUnitsGameplayBattleDummy.ContainsKey(loBoardUnit.UnitStoredPropertiesId)) continue;
                        UnitGameplayBattle loUnitGameplayBattle = goUnitsGameplayBattleDummy[loBoardUnit.UnitStoredPropertiesId];

                        UnitStoredProperties loInvoker = GetUnitStoredProperties(loUnitGameplayBattle.InvokedByUnitStoredPropertiesId.Value);

                        //<< Si no se encontró no aplicamos.
                        if (loInvoker == null) continue;

                        int lnCellIndexRefresh = goBoard.CellIndexOf(loInvoker.X, loInvoker.Z);
                        BoardCell<BoardUnit> loCell = goBoard.GetCell(lnCellIndexRefresh);
                        loInvoker.UnitInstance.transform.position = loCell.Vector;
                    }
                    break;

                case ClientCommand.PlayPlayablePrefabCoroutine:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Reproducimos.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Reproducimos coroutine de los targets de ser previamente asignados.
                        if (loGameplayEffectAreaCells != null) yield return loPlayablePrefab.PlayCoroutine(loInstruction.Value, lnGameplayEffectAreaSections, loGameplayEffectAreaCells);
                        else yield return loPlayablePrefab.PlayCoroutine(loInstruction.Value, 1, goBoard.GetCell(lnCellIndexTarget));
                    }
                    break;

                case ClientCommand.PlayPlayablePrefabCoroutineAsynchronous:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Reproducimos.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        if (loGameplayEffectAreaCells != null) StartCoroutine(loPlayablePrefab.PlayCoroutine(loInstruction.Value, lnGameplayEffectAreaSections, loGameplayEffectAreaCells));
                        else StartCoroutine(loPlayablePrefab.PlayCoroutine(loInstruction.Value, 1, goBoard.GetCell(lnCellIndexTarget)));
                    }
                    break;

                case ClientCommand.PlayPlayablePrefabCoroutineBySection:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        int lnSectionCells = 0;

                        //<< Reproducimos.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Obtenemos la sección a la cual aplicaremos el efecto.
                        BoardCell<BoardUnit>.GetSectionCells(lnGameplayEffectAreaSection, lnGameplayEffectAreaCells, loGameplayEffectAreaCells, loGameplayEffectAreaSectionCells, out lnSectionCells);

                        //<< Si no hay celdas del area seleccionada continuamos.
                        if (lnSectionCells == 0) continue;

                        //<< Reproducimos el efecto visual a la sección del area.
                        if (loPlayablePrefab != null) yield return loPlayablePrefab.PlayCoroutineBySection(loInstruction.Value, lnGameplayEffectAreaSections, lnGameplayEffectAreaSection, loGameplayEffectAreaSectionCells, lnSectionCells);
                    }
                    break;

                case ClientCommand.SetValuePlayablePrefab:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Obtenemos el elemento de la lista.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Asignamos el valor.
                        loPlayablePrefab.SetValue(loInstruction.Value);
                    }
                    break;

                case ClientCommand.SetValueFromEffectPlayablePrefab:
                    {
                        //<< Obtenemos el GameplayEffect.
                        GameplayEffect loGameplayEffectIn = goGameplayEffects[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Obtenemos el elemento de la lista.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Asignamos el valor que viene del efecto.
                        loPlayablePrefab.SetValue(loGameplayEffectIn.Value.ToString("+#;-#;0"));
                    }
                    break;

                case ClientCommand.SetValuePlayablePrefabAnimator:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Obtenemos el elemento de la lista.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Asignamos el valor del animator que nos piden.
                        loPlayablePrefab.SetValue(loBoardUnit.UnitInstance.animator);
                    }
                    break;
                case ClientCommand.SetValuePlayablePrefabAnimationTrigger:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Obtenemos el elemento de la lista.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];

                        //<< Asignamos el valor del trigger.
                        loPlayablePrefab.SetValue(lnAnimationTrigger);
                    }
                    break;

                case ClientCommand.ApplyGameplayEffect:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        yield return ApplyGameplayEffect(loBoardUnit, loInstruction.GameplayEffectId.Value, lnCellIndexTarget, loFacingTarget);
                    }
                    break;

                case ClientCommand.RemoveGameplayEffect:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        if (loInstruction.OnTarget)
                        {
                            if (loGameplayEffectAreaCells == null) continue;

                            int lnUnits;
                            BoardUnit loItem;
                            BoardCell<BoardUnit> loCell;
                            for (int c = 0; c < lnGameplayEffectAreaCells; c++)
                            {
                                loCell = loGameplayEffectAreaCells[c];

                                lnUnits = loCell.Data.Count;
                                for (int j = lnUnits - 1; j >= 0; j--)
                                {
                                    loItem = loCell.Data[j];
                                    yield return RemoveUnitGameplayEffect(loItem, loInstruction.GameplayEffectId.Value);
                                }
                            }
                        }
                        else yield return RemoveUnitGameplayEffect(loBoardUnit, loInstruction.GameplayEffectId.Value);
                    }
                    break;

                case ClientCommand.TranslatePlayablePrefabToStoredPosition:
                    {
                        //<< Si no se pudo convertir en Id continuamos.
                        if (loInstruction.VisualEffectAssetBundleId == null) continue;

                        //<< Si no existe en la lista continuamos.
                        if (!loPlayablePrefabs.ContainsKey(loInstruction.VisualEffectAssetBundleId.Value)) continue;

                        //<< Movemos el playable prefab a la posicion almacenada.
                        PlayablePrefab loPlayablePrefab = loPlayablePrefabs[loInstruction.VisualEffectAssetBundleId.Value];
                        loPlayablePrefab.transform.position = goStoredPosition;
                    }
                    break;

                case ClientCommand.IncrementAreaSection:
                    if (!string.IsNullOrWhiteSpace(loInstruction.Value) && int.TryParse(loInstruction.Value, out int lnIncrement)) lnGameplayEffectAreaSection += lnIncrement;
                    else lnGameplayEffectAreaSection++;
                    break;

                #region CONDITIONALS
                case ClientCommand.IfAreaSectionsNotCompletedReturnNCommands:
                    if (!string.IsNullOrWhiteSpace(loInstruction.Value) && int.TryParse(loInstruction.Value, out int lnBacks))
                    {
                        if (lnGameplayEffectAreaSection < lnGameplayEffectAreaSections) i -= lnBacks;
                    }
                    break;

                case ClientCommand.SkipInstructionsIfGameplayTargetIsInCell:
                    {
                        if (!int.TryParse(loInstruction.Value, out int lnValue)) continue;
                        GameplayTarget loGameplayTarget = (GameplayTarget)lnValue;
                        if (loGameplayTarget == GameplayTarget.None) continue;

                        BoardCell<BoardUnit> loCell = goBoard.GetCell(lnCellIndexTarget);
                        int lnUnits = loCell.Data.Count;
                        for (int j = lnUnits - 1; j >= 0; j--)
                        {
                            //<< Si es objetivo salimos de la coroutina ignorando el resto de intrucciones.
                            if (IsTarget(loCell.Data[j], loBoardUnit, loGameplayTarget)) yield break;
                        }
                    }
                    break;

                case ClientCommand.SkipInstructionsIfGameplayTargetIsNotInCell:
                    {
                        if (!int.TryParse(loInstruction.Value, out int lnValue)) continue;
                        GameplayTarget loGameplayTarget = (GameplayTarget)lnValue;
                        if (loGameplayTarget == GameplayTarget.None) continue;

                        BoardCell<BoardUnit> loCell = goBoard.GetCell(lnCellIndexTarget);
                        int lnUnits = loCell.Data.Count;
                        bool lbIsInCell = false;
                        for (int j = lnUnits - 1; j >= 0; j--)
                        {
                            //<< Si es objetivo salimos de la coroutina ignorando el resto de intrucciones.
                            if (!IsTarget(loCell.Data[j], loBoardUnit, loGameplayTarget)) continue;
                            lbIsInCell = true;
                            break;
                        }

                        if (!lbIsInCell) yield break;
                    }
                    break;

                case ClientCommand.SkipInstructionsIfInstructionCondition:
                    {
                        if (!int.TryParse(loInstruction.Value, out int lnValue)) continue;
                        InstructionCondition loInstructionCondition = (InstructionCondition)lnValue;
                        switch (loInstructionCondition)
                        {
                            case InstructionCondition.IgnoreWhenCallerIsHidden:
                                if (loBoardUnit.UnitGameplayEffects.Contains(GameplayEffectType.UnitHide)) yield break;
                                break;
                            case InstructionCondition.IgnoreWhenDamageHasArea:
                                if (loGameplayEffect.Area > 1) yield break;
                                break;
                            case InstructionCondition.IgnoreWhenCallerPushes:
                                {
                                    //<< Obtenemos el evento de ataque para ver si es del tipo de empuje.
                                    UnitEvent loUnitEventAttack = GetUnitEvent(loBoardUnit.UnitProperties.UnitBattleProperties.Id, UnitEventTiming.Attack);

                                    //<< Si no tenemos evento de ataque continuamos.
                                    if (loUnitEventAttack == null) continue;

                                    //<< Obtenemos las instrucciones del evento para saber si alguno de los efectos empuja.
                                    UnitEventInstruction[] loUnitEventInstructionsIn = goUnitEventInstructions.Values.Where(x => x.UnitEventId == loUnitEventAttack.UnitEventId && x.Command == ClientCommand.ApplyGameplayEffect).ToArray();

                                    //<< Revisamos si tenemos alguno de empuje.
                                    bool lbPushes = false;
                                    foreach (UnitEventInstruction loUnitEventInstruction in loUnitEventInstructionsIn)
                                    {
                                        if (loUnitEventInstruction.GameplayEffectId == null) continue;
                                        if (goGameplayEffects[loUnitEventInstruction.GameplayEffectId.Value].Type != GameplayEffectType.InvokeUnitEventPushAsync) continue;
                                        lbPushes = true;
                                        break;
                                    }

                                    //<< Como si tiene empuje nos salimos.
                                    if (lbPushes) yield break;
                                }
                                break;
                            default: continue;
                        }
                    }
                    break;

                case ClientCommand.SkipInstructionsIfCallerIsGameplayTarget:
                    {
                        if (!int.TryParse(loInstruction.Value, out int lnValue)) continue;
                        GameplayTarget loGameplayTarget = (GameplayTarget)lnValue;
                        if (loGameplayTarget == GameplayTarget.None) continue;

                        //<< Si no es objetivo salimos de la coroutina ignorando el resto de intrucciones.
                        if (IsTarget(loBoardUnitCaller, loBoardUnit, loGameplayTarget)) yield break;
                    }
                    break;

                case ClientCommand.SkipInstructionIfCallerIsNotGameplayTarget:
                    {
                        if (!int.TryParse(loInstruction.Value, out int lnValue)) continue;
                        GameplayTarget loGameplayTarget = (GameplayTarget)lnValue;
                        if (loGameplayTarget == GameplayTarget.None) continue;

                        //<< Si no es objetivo salimos de la coroutina ignorando el resto de intrucciones.
                        if (!IsTarget(loBoardUnitCaller, loBoardUnit, loGameplayTarget)) yield break;
                    }
                    break;

                case ClientCommand.EvaluateCriticalFromInstructions:
                    loBoardUnitCaller.Critical = EvaluateCriticalFromInstructions(loInstructions, lnInstructionsCount, i);
                    break;
                case ClientCommand.EvaluateCriticalFromBackstab:
                    {
                        //<< Si estamos viendo en la misma dirección sabemo que va a ser por la espalda, de lo contrario no.
                        BoardCell<BoardUnit> loCell = goBoard.GetCell(lnCellIndexTarget);
                        int lnUnits = loCell.Data.Count;
                        for (int j = lnUnits - 1; j >= 0; j--)
                        {
                            //<< Si es objetivo salimos de la coroutina ignorando el resto de intrucciones.
                            if (!IsTarget(loCell.Data[j], loBoardUnit, GameplayTarget.Unit | GameplayTarget.GameplayUnit)) continue;
                            loBoardUnitCaller.Critical = loBoardUnitCaller.Facing == loCell.Data[j].Facing;
                            if (loBoardUnitCaller.Critical) break;
                        }
                    }
                    break;
                case ClientCommand.SkipInstructionsIfCritical:
                    {
                        if (loBoardUnitCaller.Critical && !string.IsNullOrWhiteSpace(loInstruction.Value) && int.TryParse(loInstruction.Value, out int lnSkips)) i += lnSkips;
                    }
                    break;
                case ClientCommand.SkipInstructionsIfNotCritical:
                    {
                        if (!loBoardUnitCaller.Critical && !string.IsNullOrWhiteSpace(loInstruction.Value) && int.TryParse(loInstruction.Value, out int lnSkips)) i += lnSkips;
                    }
                    break;

                case ClientCommand.SkipInstructionsIfOpponent:
                    {
                        if (!loBoardUnit.Opponent) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;
                        i += lnSkipInstructionsCount;
                    }
                    break;
                case ClientCommand.SkipInstructionsIfNotOpponent:
                    {
                        if (loBoardUnit.Opponent) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;
                        i += lnSkipInstructionsCount;
                    }
                    break;
                case ClientCommand.SkipInstructionsIfGameplayEffectIdExists:
                    {
                        //<< Si no hay effect id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loBoardUnit.GameplayEffects.Any(x => x.Id == loInstruction.GameplayEffectId.Value)) i += lnSkipInstructionsCount;

                    }
                    break;

                case ClientCommand.SkipInstructionsIfGameplayEffectIdNotExists:
                    {
                        //<< Si no hay effect id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (!loBoardUnit.GameplayEffects.Any(x => x.Id == loInstruction.GameplayEffectId.Value)) i += lnSkipInstructionsCount;
                    }
                    break;

                case ClientCommand.SkipInstructionsIfGameplayEffectIdAdded:
                    {
                        if (loUnitEventTiming != UnitEventTiming.GameplayEffectAdded) continue;

                        //<< Si no hay effect id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loGameplayEffectCaller.Id == loInstruction.GameplayEffectId.Value) i += lnSkipInstructionsCount;
                    }
                    break;
                case ClientCommand.SkipInstructionsIfGameplayEffectIdRemoved:
                    {
                        if (loUnitEventTiming != UnitEventTiming.GameplayEffectRemoved) continue;

                        //<< Si no hay effect id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loGameplayEffectCaller.Id == loInstruction.GameplayEffectId.Value) i += lnSkipInstructionsCount;
                    }
                    break;
                case ClientCommand.SkipInstructionsIfGameplayEffectIdNotAdded:
                    {
                        if (loUnitEventTiming != UnitEventTiming.GameplayEffectAdded) continue;

                        //<< Si no hay effect id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loGameplayEffectCaller.Id != loInstruction.GameplayEffectId.Value) i += lnSkipInstructionsCount;
                    }
                    break;
                case ClientCommand.SkipInstructionsIfGameplayEffectIdNotRemoved:
                    {
                        if (loUnitEventTiming != UnitEventTiming.GameplayEffectRemoved) continue;

                        //<< Si no hay effect id continuamos.
                        if (loInstruction.GameplayEffectId == null) continue;

                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loGameplayEffectCaller.Id != loInstruction.GameplayEffectId.Value) i += lnSkipInstructionsCount;
                    }
                    break;

                case ClientCommand.SkipInstructionsIfEnergyFull:
                    {
                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loBoardUnitCaller.Energy >= loBoardUnitCaller.GetMaxEnergy()) i += lnSkipInstructionsCount;
                    }
                    break;

                case ClientCommand.SkipInstructionsIfEnergyNotFull:
                    {
                        //<< Si no se puede convertir continuamos.
                        if (!int.TryParse(loInstruction.Value, out int lnSkipInstructionsCount)) continue;

                        //<< Revisamos si tenemos el id dado, de ser así saltamos las instrucciones dadas.
                        if (loBoardUnitCaller.Energy < loBoardUnitCaller.GetMaxEnergy()) i += lnSkipInstructionsCount;
                    }
                    break;
                #endregion

                #region SOUND
                case ClientCommand.PlaySound:
                    if (!string.IsNullOrWhiteSpace(loInstruction.Value)) RuntimeManager.PlayOneShot(loInstruction.Value);
                    break;

                case ClientCommand.PlaySound_C0:
                    if (!string.IsNullOrWhiteSpace(loInstruction.Value))
                    {
                        if (goFMODInstance_C0.hasHandle())
                        {
                            goFMODInstance_C0.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                            goFMODInstance_C0.release();
                        }

                        goFMODInstance_C0 = FMODUnity.RuntimeManager.CreateInstance(loInstruction.Value);
                        goFMODInstance_C0.start();
                    }
                    break;
                case ClientCommand.PauseSound_C0:
                    if (goFMODInstance_C0.hasHandle() && !string.IsNullOrWhiteSpace(loInstruction.Value) && bool.TryParse(loInstruction.Value, out bool lbPause_C0))
                    {
                        goFMODInstance_C0.setPaused(lbPause_C0);
                    }
                    break;
                case ClientCommand.StopSound_C0:
                    if (goFMODInstance_C0.hasHandle())
                    {
                        goFMODInstance_C0.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        goFMODInstance_C0.release();
                    }
                    break;

                case ClientCommand.PlaySound_C1:
                    if (!string.IsNullOrWhiteSpace(loInstruction.Value))
                    {
                        if (goFMODInstance_C1.hasHandle())
                        {
                            goFMODInstance_C1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                            goFMODInstance_C1.release();
                        }

                        goFMODInstance_C1 = FMODUnity.RuntimeManager.CreateInstance(loInstruction.Value);
                        goFMODInstance_C1.start();
                    }
                    break;
                case ClientCommand.PauseSound_C1:
                    if (goFMODInstance_C1.hasHandle() && !string.IsNullOrWhiteSpace(loInstruction.Value) && bool.TryParse(loInstruction.Value, out bool lbPause_C1))
                    {
                        goFMODInstance_C1.setPaused(lbPause_C1);
                    }
                    break;
                case ClientCommand.StopSound_C1:
                    if (goFMODInstance_C1.hasHandle())
                    {
                        goFMODInstance_C1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        goFMODInstance_C1.release();
                    }
                    break;
                #endregion

                #region TRANSFORM_ANIMATION
                case ClientCommand.AddStoredVector:
                    {
                        //<< Convertimos value a StoredVector.
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;
                        string[] loValues = loInstruction.Value.Split(",");
                        if (loValues.Length != 5) continue;

                        if (!float.TryParse(loValues[1], out float lnN)) continue;
                        if (!float.TryParse(loValues[2], out float lnX)) continue;
                        if (!float.TryParse(loValues[3], out float lnY)) continue;
                        if (!float.TryParse(loValues[4], out float lnZ)) continue;

                        StoredVector loStoredVector = new StoredVector();
                        loStoredVector.T = loValues[0][0];
                        loStoredVector.N = lnN;
                        loStoredVector.V.x = lnX;
                        loStoredVector.V.y = lnY;
                        loStoredVector.V.z = lnZ;
                        loStoredVectors.Add(loStoredVector);
                    }
                    break;
                case ClientCommand.ClearStoredVectors: loStoredVectors.Clear(); break;

                case ClientCommand.AddStoredPositionToStoredVectors:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;
                        if (!float.TryParse(loInstruction.Value, out float lnN)) continue;
                        loStoredVectors.Add(new StoredVector() { V = goStoredPosition, N = lnN, T = 'p' });
                    }
                    break;

                case ClientCommand.ConvertPositionStoredVectorsToBezier:
                    {
                        int lnExtraPointsPerSection = 1;
                        if (!string.IsNullOrWhiteSpace(loInstruction.Value)) int.TryParse(loInstruction.Value, out lnExtraPointsPerSection);

                        IEnumerable<StoredVector> loFiltered = loStoredVectors.Where(x => x.T == 'p').ToList();
                        loStoredVectors.RemoveAll(x => x.T == 'p');
                        loStoredVectors.AddRange(ConvertStoredVectorsToBezier(loFiltered, lnExtraPointsPerSection));
                    }
                    break;

                case ClientCommand.AnimateStoredVectorsOnStoredTransformLinear:
                case ClientCommand.AnimateStoredVectorsOnStoredTransformEased:
                case ClientCommand.AnimateStoredVectorsOnStoredTransformGameManagerCurve:
                    {
                        if (!float.TryParse(loInstruction.Value, out float lnTime)) continue;
                        if (goStoredTransform == null) continue;

                        if (loInstruction.Command == ClientCommand.AnimateStoredVectorsOnStoredTransformLinear)
                            yield return AnimateStoredVectorsGPT(goStoredTransform, loStoredVectors, AnimationCurve.Linear(0, 0, 1, 1), lnTime);
                        else if (loInstruction.Command == ClientCommand.AnimateStoredVectorsOnStoredTransformEased) yield return AnimateStoredVectorsGPT(goStoredTransform, loStoredVectors, AnimationCurve.EaseInOut(0, 0, 1, 1), lnTime);
                        else AnimateStoredVectorsGPT(goStoredTransform, loStoredVectors, theaterAnimationCurve, lnTime);
                    }
                    break;
                case ClientCommand.ResetStoredTransform:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;

                        if (goStoredTransform == null) continue;

                        goStoredTransform.localScale = Vector3.one;
                        goStoredTransform.localPosition = Vector3.zero;
                        goStoredTransform.localRotation = Quaternion.identity;
                    }
                    break;
                #endregion

                #region RENDERER_MATERIAL
                case ClientCommand.SetRendererMaterialColorAsHealthbarColor:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;

                        //<< Obtenemos el transform de la unidad al cual le debemos cambiar el color.
                        Transform loTransform = loBoardUnit.UnitInstance.transform.FindNearest(loInstruction.Value);

                        if (loTransform == null) continue;

                        Renderer[] loRenderers = loTransform.GetComponentsInChildren<Renderer>(true);

                        if (loRenderers == null || loRenderers.Length == 0) continue;

                        //<< Obtenemos de quien es la unidad.
                        MaterialPropertyBlock loMaterialPropertyBlock = new MaterialPropertyBlock();
                        Color loColor;
                        if (gnUserId == loBoardUnit.UserId) loColor = unitOutlineColorUser;
                        else loColor = unitOutlineColorSecundary;

                        foreach (Renderer loRenderer in loRenderers)
                        {
                            loRenderer.GetPropertyBlock(loMaterialPropertyBlock);
                            loMaterialPropertyBlock.SetColor("_BaseColor", loColor);
                            loRenderer.SetPropertyBlock(loMaterialPropertyBlock);
                        }
                    }
                    break;
                #endregion

                #region SHADER
                case ClientCommand.SetShaderPropertyMPB:
                    {
                        //<< Obtenemos todos los parametros necesarios.
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;

                        //<< Revisamos que el nombre de parametro inicie con el guion bajo.
                        if (!loInstruction.Value.StartsWith('_')) continue;

                        //<< Dividimos en parametros, si no son 3 continuamos por que viene mal.
                        string[] loParams = loInstruction.Value.Split('$');
                        if (loParams.Length != 3) continue;

                        int lnCount;
                        string lcParamName = loParams[0];
                        string lcParamType = loParams[1];
                        string lcParamValue = loParams[2];
                        MaterialPropertyBlock loMPB = new MaterialPropertyBlock();
                        switch (lcParamType)
                        {
                            case "float":
                                {
                                    if (!float.TryParse(lcParamValue, out float lnFloat)) continue;
                                    foreach (Renderer loRenderer in loBoardUnit.UnitInstance.renderers)
                                    {
                                        lnCount = loRenderer.materials.Length;
                                        for (int m = 0; m < lnCount; m++)
                                        {
                                            loRenderer.GetPropertyBlock(loMPB, m);
                                            loMPB.SetFloat(lcParamName, lnFloat);
                                            loRenderer.SetPropertyBlock(loMPB, m);
                                        }
                                    }

                                    foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                                    {
                                        foreach (Renderer loRenderer in loUnitElement.renderers)
                                        {
                                            lnCount = loRenderer.materials.Length;
                                            for (int m = 0; m < lnCount; m++)
                                            {
                                                loRenderer.GetPropertyBlock(loMPB, m);
                                                loMPB.SetFloat(lcParamName, lnFloat);
                                                loRenderer.SetPropertyBlock(loMPB, m);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "vector":
                                {
                                    if (!TryParseToVector3(lcParamValue, out Vector3 loVector3)) continue;
                                    foreach (Renderer loRenderer in loBoardUnit.UnitInstance.renderers)
                                    {
                                        lnCount = loRenderer.materials.Length;
                                        for (int m = 0; m < lnCount; m++)
                                        {
                                            loRenderer.GetPropertyBlock(loMPB, m);
                                            loMPB.SetVector(lcParamName, loVector3);
                                            loRenderer.SetPropertyBlock(loMPB, m);
                                        }
                                    }

                                    foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                                    {
                                        foreach (Renderer loRenderer in loUnitElement.renderers)
                                        {
                                            lnCount = loRenderer.materials.Length;
                                            for (int m = 0; m < lnCount; m++)
                                            {
                                                loRenderer.GetPropertyBlock(loMPB, m);
                                                loMPB.SetVector(lcParamName, loVector3);
                                                loRenderer.SetPropertyBlock(loMPB, m);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "color":
                                {
                                    if (!ColorUtility.TryParseHtmlString(lcParamValue, out Color loColor)) continue;
                                    foreach (Renderer loRenderer in loBoardUnit.UnitInstance.renderers)
                                    {
                                        lnCount = loRenderer.materials.Length;
                                        for (int m = 0; m < lnCount; m++)
                                        {
                                            loRenderer.GetPropertyBlock(loMPB, m);
                                            loMPB.SetColor(lcParamName, loColor);
                                            loRenderer.SetPropertyBlock(loMPB, m);
                                        }
                                    }

                                    foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                                    {
                                        foreach (Renderer loRenderer in loUnitElement.renderers)
                                        {
                                            lnCount = loRenderer.materials.Length;
                                            for (int m = 0; m < lnCount; m++)
                                            {
                                                loRenderer.GetPropertyBlock(loMPB, m);
                                                loMPB.SetColor(lcParamName, loColor);
                                                loRenderer.SetPropertyBlock(loMPB, m);
                                            }
                                        }
                                    }
                                }
                                break;
                            default: continue;
                        }
                    }
                    break;

                case ClientCommand.SetShaderProperty:
                    {
                        //<< Obtenemos todos los parametros necesarios.
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;

                        //<< Revisamos que el nombre de parametro inicie con el guion bajo.
                        if (!loInstruction.Value.StartsWith('_')) continue;

                        //<< Dividimos en parametros, si no son 3 continuamos por que viene mal.
                        string[] loParams = loInstruction.Value.Split('$');
                        if (loParams.Length != 3) continue;

                        string lcParamName = loParams[0];
                        string lcParamType = loParams[1];
                        string lcParamValue = loParams[2];

                        switch (lcParamType)
                        {
                            case "float":
                                {
                                    if (!float.TryParse(lcParamValue, out float lnFloat)) continue;
                                    Material[][] loMaterialInstances = loBoardUnit.UnitInstance.GetMaterialInstances();
                                    foreach (Material[] loMaterials in loMaterialInstances)
                                    {
                                        foreach (Material loMaterial in loMaterials) loMaterial.SetFloat(lcParamName, lnFloat);
                                    }

                                    foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                                    {
                                        loMaterialInstances = loUnitElement.GetMaterialInstances();
                                        foreach (Material[] loMaterials in loMaterialInstances)
                                        {
                                            foreach (Material loMaterial in loMaterials) loMaterial.SetFloat(lcParamName, lnFloat);
                                        }
                                    }
                                }
                                break;
                            case "vector":
                                {
                                    if (!TryParseToVector3(lcParamValue, out Vector3 loVector3)) continue;
                                    Material[][] loMaterialInstances = loBoardUnit.UnitInstance.GetMaterialInstances();
                                    foreach (Material[] loMaterials in loMaterialInstances)
                                    {
                                        foreach (Material loMaterial in loMaterials) loMaterial.SetVector(lcParamName, loVector3);
                                    }

                                    foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                                    {
                                        loMaterialInstances = loUnitElement.GetMaterialInstances();
                                        foreach (Material[] loMaterials in loMaterialInstances)
                                        {
                                            foreach (Material loMaterial in loMaterials) loMaterial.SetVector(lcParamName, loVector3);
                                        }
                                    }
                                }
                                break;
                            case "color":
                                {
                                    if (!ColorUtility.TryParseHtmlString(lcParamValue, out Color loColor)) continue;
                                    Material[][] loMaterialInstances = loBoardUnit.UnitInstance.GetMaterialInstances();
                                    foreach (Material[] loMaterials in loMaterialInstances)
                                    {
                                        foreach (Material loMaterial in loMaterials) loMaterial.SetColor(lcParamName, loColor);
                                    }

                                    foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                                    {
                                        loMaterialInstances = loUnitElement.GetMaterialInstances();
                                        foreach (Material[] loMaterials in loMaterialInstances)
                                        {
                                            foreach (Material loMaterial in loMaterials) loMaterial.SetColor(lcParamName, loColor);
                                        }
                                    }
                                }
                                break;
                            default: continue;
                        }
                    }
                    break;

                case ClientCommand.SetMaterialRenderQueue:
                    {
                        if (!int.TryParse(loInstruction.Value, out int lnValue)) continue;

                        Material[][] loMaterialInstances = loBoardUnit.UnitInstance.GetMaterialInstances();
                        foreach (Material[] loMaterials in loMaterialInstances)
                        {
                            foreach (Material loMaterial in loMaterials) loMaterial.renderQueue = lnValue;
                        }

                        foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                        {
                            loMaterialInstances = loUnitElement.GetMaterialInstances();
                            foreach (Material[] loMaterials in loMaterialInstances)
                            {
                                foreach (Material loMaterial in loMaterials) loMaterial.renderQueue = lnValue;
                            }
                        }
                    }
                    break;

                case ClientCommand.MaterialEnableKeyword:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;
                        Material[][] loMaterialInstances = loBoardUnit.UnitInstance.GetMaterialInstances();
                        foreach (Material[] loMaterials in loMaterialInstances)
                        {
                            foreach (Material loMaterial in loMaterials) loMaterial.EnableKeyword(loInstruction.Value);
                        }

                        foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                        {
                            loMaterialInstances = loUnitElement.GetMaterialInstances();
                            foreach (Material[] loMaterials in loMaterialInstances)
                            {
                                foreach (Material loMaterial in loMaterials) loMaterial.EnableKeyword(loInstruction.Value);
                            }
                        }
                    }
                    break;

                case ClientCommand.MaterialDisableKeyword:
                    {
                        if (string.IsNullOrWhiteSpace(loInstruction.Value)) continue;

                        Material[][] loMaterialInstances = loBoardUnit.UnitInstance.GetMaterialInstances();
                        foreach (Material[] loMaterials in loMaterialInstances)
                        {
                            foreach (Material loMaterial in loMaterials) loMaterial.DisableKeyword(loInstruction.Value);
                        }

                        foreach (UnitElement loUnitElement in loBoardUnit.UnitInstance.UnitElements)
                        {
                            loMaterialInstances = loUnitElement.GetMaterialInstances();
                            foreach (Material[] loMaterials in loMaterialInstances)
                            {
                                foreach (Material loMaterial in loMaterials) loMaterial.DisableKeyword(loInstruction.Value);
                            }
                        }
                    }
                    break;
                #endregion

                default: break;
            }
        }

        yield break;
    }

    private static IEnumerator AnimateStoredVectorsGPT(Transform loTransform, IEnumerable<StoredVector> loStoredVectors, AnimationCurve loAnimationCurve, float lnTime)
    {
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        float startTime = Time.time;
        float endTime = startTime + lnTime;

        //=== Clasificar por tipo ===
        List<StoredVector> loPositions = loStoredVectors.Where(x => x.T == 'p').OrderBy(x => x.N).ToList();
        List<StoredVector> loRotations = loStoredVectors.Where(x => x.T == 'r').OrderBy(x => x.N).ToList();
        List<StoredVector> loScales = loStoredVectors.Where(x => x.T == 's').OrderBy(x => x.N).ToList();

        //=== Insertar valores iniciales (si faltan) ===
        if (loPositions.Count == 0 || loPositions[0].N > 0f) loPositions.Insert(0, new StoredVector { N = 0f, V = loTransform.localPosition });

        if (loRotations.Count == 0 || loRotations[0].N > 0f) loRotations.Insert(0, new StoredVector { N = 0f, V = loTransform.localRotation.eulerAngles });

        if (loScales.Count == 0 || loScales[0].N > 0f) loScales.Insert(0, new StoredVector { N = 0f, V = loTransform.localScale });

        //=== Insertar valores finales (si faltan) ===
        if (loPositions[^1].N < 1f) loPositions.Add(new StoredVector { N = 1f, V = loTransform.localPosition });

        if (loRotations[^1].N < 1f) loRotations.Add(new StoredVector { N = 1f, V = loTransform.localRotation.eulerAngles });

        if (loScales[^1].N < 1f) loScales.Add(new StoredVector { N = 1f, V = loTransform.localScale });

        //=== Índices y referencias ===
        int lnIndexPosition = 0, lnIndexRotation = 0, lnIndexScale = 0;

        StoredVector loPositionCurrent = loPositions[0];
        StoredVector loPositionNext = loPositions[1];
        float lnSectionNormalizedPosition = loPositionNext.N - loPositionCurrent.N;

        StoredVector loRotationCurrent = loRotations[0];
        StoredVector loRotationNext = loRotations[1];
        float lnSectionNormalizedRotation = loRotationNext.N - loRotationCurrent.N;

        StoredVector loScaleCurrent = loScales[0];
        StoredVector loScaleNext = loScales[1];
        float lnSectionNormalizedScale = loScaleNext.N - loScaleCurrent.N;

        //=== Loop principal ===
        while (Time.time < endTime)
        {
            float lnDelta = Mathf.Clamp01((Time.time - startTime) / lnTime);

            //--- Avanzar índices si la sección se completó ---
            if (lnDelta > loPositionNext.N && lnIndexPosition + 1 < loPositions.Count - 1)
            {
                lnIndexPosition++;
                loPositionCurrent = loPositions[lnIndexPosition];
                loPositionNext = loPositions[lnIndexPosition + 1];
                lnSectionNormalizedPosition = loPositionNext.N - loPositionCurrent.N;
            }

            if (lnDelta > loRotationNext.N && lnIndexRotation + 1 < loRotations.Count - 1)
            {
                lnIndexRotation++;
                loRotationCurrent = loRotations[lnIndexRotation];
                loRotationNext = loRotations[lnIndexRotation + 1];
                lnSectionNormalizedRotation = loRotationNext.N - loRotationCurrent.N;
            }

            if (lnDelta > loScaleNext.N && lnIndexScale + 1 < loScales.Count - 1)
            {
                lnIndexScale++;
                loScaleCurrent = loScales[lnIndexScale];
                loScaleNext = loScales[lnIndexScale + 1];
                lnSectionNormalizedScale = loScaleNext.N - loScaleCurrent.N;
            }

            //--- Interpolaciones con curva ---
            float tPos = loAnimationCurve.Evaluate(Mathf.InverseLerp(loPositionCurrent.N, loPositionNext.N, lnDelta));
            Vector3 loPosition = Vector3.Lerp(loPositionCurrent.V, loPositionNext.V, tPos);

            float tRot = loAnimationCurve.Evaluate(Mathf.InverseLerp(loRotationCurrent.N, loRotationNext.N, lnDelta));
            Quaternion loRotation = Quaternion.Lerp(Quaternion.Euler(loRotationCurrent.V), Quaternion.Euler(loRotationNext.V), tRot);

            float tScl = loAnimationCurve.Evaluate(Mathf.InverseLerp(loScaleCurrent.N, loScaleNext.N, lnDelta));
            Vector3 loScale = Vector3.Lerp(loScaleCurrent.V, loScaleNext.V, tScl);

            //--- Aplicar valores ---
            loTransform.localScale = loScale;
            loTransform.SetLocalPositionAndRotation(loPosition, loRotation);

            yield return loWaitForEndOfFrame;
        }

        //=== Forzar frame final exacto ===
        loTransform.localScale = loScales[^1].V;
        loTransform.SetLocalPositionAndRotation(loPositions[^1].V, Quaternion.Euler(loRotations[^1].V));
    }

    private static List<StoredVector> ConvertStoredVectorsToBezier(IEnumerable<StoredVector> loOriginal, int lnExtraPointsPerSection)
    {
        List<StoredVector> loSorted = loOriginal.OrderBy(o => o.N).ToList();
        List<StoredVector> loResult = new List<StoredVector>();

        //<< Si no tiene suficientes vectores...
        if (loSorted.Count < 2) return loSorted;

        StoredVector p0, p1, p2, p3;

        int lnCount = loSorted.Count - 1;
        for (int i = 0; i < lnCount; i++)
        {
            p0 = i == 0 ? loSorted[i] : loSorted[i - 1];
            p1 = loSorted[i];
            p2 = loSorted[i + 1];
            p3 = i + 2 < loSorted.Count ? loSorted[i + 2] : loSorted[i + 1];

            // Convertimos Catmull–Rom en Bézier
            Vector3 b0 = p1.V;
            Vector3 b3 = p2.V;

            Vector3 b1 = p1.V + (p2.V - p0.V) / 6f;
            Vector3 b2 = p2.V - (p3.V - p1.V) / 6f;

            // Agregar punto inicial del tramo
            loResult.Add(p1);

            // Insertar puntos internos en la curva
            for (int k = 1; k <= lnExtraPointsPerSection; k++)
            {
                float t = (float)k / (lnExtraPointsPerSection + 1);

                float nt = Mathf.Lerp(p1.N, p2.N, t);

                Vector3 v =
                    Mathf.Pow(1 - t, 3) * b0 +
                    3 * Mathf.Pow(1 - t, 2) * t * b1 +
                    3 * (1 - t) * Mathf.Pow(t, 2) * b2 +
                    Mathf.Pow(t, 3) * b3;

                loResult.Add(new StoredVector { T = p1.T, N = nt, V = v });
            }
        }

        //<< Agregamos el último punto.
        loResult.Add(loSorted[loSorted.Count - 1]);

        return loResult;
    }

    /// <summary>
    /// Se llama cuando la unidad seleccionada realiza un movimiento por medio del "RootMotion" en las celdas.
    /// </summary>
    private void OnUnitAnimatorMoveOnCells(UnitInstance loUnitInstance)
    {
        //<< Determinamos la velocidad de la unidad.
        gnDeltaCellPath += loUnitInstance.animator.deltaPosition.magnitude;

        //<< Deseleccionamos nuestra instancia.
        if (gnDeltaCellPath >= goUnitCellsSelected.Count - 1)
        {
            EndUnitInstanceMove(loUnitInstance);
            return;
        }

        Vector3 loPosition = loUnitInstance.transform.position;
        Quaternion loRotation = loUnitInstance.transform.rotation;

        //<< Determinamos la posicion con base en el delta.
        if (!GetDisplacementAndOrientation(gnDeltaCellPath, goUnitCellsSelected, ref loPosition, ref loRotation)) return;

        loUnitInstance.transform.position = loPosition;
        loUnitInstance.transform.rotation = loRotation;
    }

    /// <summary>
    /// Nos da la dirección que debemos tomar con base en la lista de celdas.
    /// </summary>
    /// <param name="lnDeltaPath"></param>
    /// <param name="loList"></param>
    /// <returns></returns>
    public static bool GetDisplacementAndOrientationBackward(float lnDeltaPath, IList<BoardCell<T>> loList, ref Vector3 loPosition, ref Quaternion loRotation)
    {
        //<< Determinamos entre que celda y que celda nos encontramos con base en el delta.
        int lnCellStart = Mathf.CeilToInt(lnDeltaPath);
        int lnCellEnd = lnCellStart - 1;

        //<< Si es la misma celda solo regresamos.
        if (lnCellStart >= loList.Count) return false;
        if (lnCellEnd < 0) return false;

        //<< Obtenemos la posicion normalizada entre celdas.
        float lnNormalizedDistanceBetwenCells = lnCellStart - lnDeltaPath;

        BoardCell<T> loCellStart = loList[lnCellStart];
        BoardCell<T> loCellEnd = loList[lnCellEnd];

        //<< Si estamos sobre el mismo eje en X sabemos que debemos movernos sobre Z.
        if (loCellStart.X == loCellEnd.X)
        {
            if (loCellStart.Z < loCellEnd.Z) loRotation = Forward; //<< Forward
            else loRotation = Backward; //<< Backward
        }
        else//<< Si estamos sobre el mismo eje Z sabemos que debemos movernos sobre X.
        {
            if (loCellStart.X < loCellEnd.X) loRotation = Right;//<< Right
            else loRotation = Left; //<< Left
        }

        //<< Determinamos si hay una curva y si esta es la primera parte de la curva.
        int lnCellNext = lnCellEnd - 1;
        if (lnCellNext > -1 && lnNormalizedDistanceBetwenCells > 0.5f)
        {
            BoardCell<T> loCellNext = loList[lnCellNext];

            //<< Si es una curva comenzamos viaje por curva.
            if (loCellStart.X != loCellNext.X && loCellStart.Z != loCellNext.Z)
            {
                //<< Ya que la curva solo aplica hasta la mitad del desplazamiento.
                float lnNormalizedOffset = lnNormalizedDistanceBetwenCells - 0.5f;

                //<< Normalizamos el desplazamiento actual, como sabemos que solo nos desplazamos la mitad de una unidad.
                float lnNormalized = lnNormalizedOffset / 0.5f;

                //<< Calculamos la posicion que debe tener para realizar la curva.
                Vector3 loDirectionStart = loCellEnd.Vector - loCellNext.Vector;
                Vector3 loDirectionEnd = loCellEnd.Vector - loCellStart.Vector;

                Quaternion loRotationStart = Quaternion.LookRotation(loDirectionStart, Vector3.up);
                Quaternion loRotationEnd = Quaternion.LookRotation(loDirectionEnd, Vector3.up);

                //<< Obtenemos el punto medio ya que es la primera parte de la curva.
                loRotationEnd = Quaternion.Lerp(loRotationStart, loRotationEnd, 0.5f);

                //<< Rotamos con base en el delta normalizado.
                loPosition = Quaternion.Lerp(loRotationStart, loRotationEnd, lnNormalized) * (Vector3.forward * 0.5f);

                //<< Determinamos el pivote de rotación.
                float lnPivotX = (loCellNext.X - loCellStart.X) * 0.5f;
                float lnPivotZ = (loCellNext.Z - loCellStart.Z) * 0.5f;

                //<< Agregamos la posicion de la celda mas el pivote.
                loPosition.x += loCellStart.X + lnPivotX;
                loPosition.z += loCellStart.Z + lnPivotZ;

                //<< Si estamos sobre el mismo eje en X sabemos que debemos movernos sobre Z.
                if (loCellEnd.X == loCellNext.X)
                {
                    if (loCellEnd.Z < loCellNext.Z) loRotationEnd = Forward; //<< Forward
                    else loRotationEnd = Backward; //<< Backward
                }
                else//<< Si estamos sobre el mismo eje Z sabemos que debemos movernos sobre X.
                {
                    if (loCellEnd.X < loCellNext.X) loRotationEnd = Right;//<< Right
                    else loRotationEnd = Left; //<< Left
                }

                //<< Asignamos la rotacion que tendrá la unidad.
                loRotation = Quaternion.Lerp(loRotation, loRotationEnd, lnNormalizedOffset);

                return true;
            }
        }

        //<< Determinamos si hay una curva y si esta es la segunda parte de la curva.
        int lnCellPrior = lnCellStart + 1;
        if (lnCellPrior < loList.Count && lnNormalizedDistanceBetwenCells < 0.5f)
        {
            BoardCell<BoardUnit> loCellPrior = loList[lnCellPrior];

            //<< Si es una curva comenzamos viaje por curva.
            if (loCellPrior.X != loCellEnd.X && loCellPrior.Z != loCellEnd.Z)
            {
                //<< Ya que la curva solo aplica hasta la mitad del desplazamiento.
                float lnNormalizedOffset = lnNormalizedDistanceBetwenCells + 0.5f;

                //<< Normalizamos el desplazamiento actual, como sabemos que solo nos desplazamos la mitad de una unidad.
                float lnNormalized = lnNormalizedDistanceBetwenCells / 0.5f;

                //<< Calculamos la posicion que debe tener para realizar la curva.
                Vector3 loDirectionStart = loCellStart.Vector - loCellEnd.Vector;
                Vector3 loDirectionEnd = loCellStart.Vector - loCellPrior.Vector;

                Quaternion loRotationStart = Quaternion.LookRotation(loDirectionStart, Vector3.up);
                Quaternion loRotationEnd = Quaternion.LookRotation(loDirectionEnd, Vector3.up);

                //<< Obtenemos el punto medio ya que es la primera parte de la curva.
                loRotationStart = Quaternion.Lerp(loRotationStart, loRotationEnd, 0.5f);

                //<< Rotamos con base en el delta normalizado.
                loPosition = Quaternion.Lerp(loRotationStart, loRotationEnd, lnNormalized) * (Vector3.forward * 0.5f);

                //<< Determinamos el pivote de rotación.
                float lnPivotX = (loCellEnd.X - loCellPrior.X) * 0.5f;
                float lnPivotZ = (loCellEnd.Z - loCellPrior.Z) * 0.5f;

                //<< Agregamos la posicion de la celda mas el pivote.
                loPosition.x += loCellPrior.X + lnPivotX;
                loPosition.z += loCellPrior.Z + lnPivotZ;

                //<< Si estamos sobre el mismo eje en X sabemos que debemos movernos sobre Z.
                if (loCellPrior.X == loCellStart.X)
                {
                    if (loCellPrior.Z < loCellStart.Z) loRotationStart = Forward; //<< Forward
                    else loRotationStart = Backward; //<< Backward
                }
                else//<< Si estamos sobre el mismo eje Z sabemos que debemos movernos sobre X.
                {
                    if (loCellPrior.X < loCellStart.X) loRotationStart = Right;//<< Right
                    else loRotationStart = Left; //<< Left
                }

                loRotation = Quaternion.Lerp(loRotationStart, loRotation, lnNormalizedOffset);

                return true;
            }
        }

        loPosition.x = Mathf.Lerp(loCellStart.X, loCellEnd.X, lnNormalizedDistanceBetwenCells);
        loPosition.z = Mathf.Lerp(loCellStart.Z, loCellEnd.Z, lnNormalizedDistanceBetwenCells);

        return true;
    }

    /// <summary>
    /// Nos da la dirección que debemos tomar con base en la lista de celdas.
    /// </summary>
    /// <param name="lnDeltaPath"></param>
    /// <param name="loList"></param>
    /// <returns></returns>
    public static bool GetDisplacementAndOrientation(float lnDeltaPath, IList<BoardCell<BoardUnit>> loList, ref Vector3 loPosition, ref Quaternion loRotation)
    {
        //<< Determinamos entre que celda y que celda nos encontramos con base en el delta.
        int lnCellStart = Mathf.FloorToInt(lnDeltaPath);
        int lnCellEnd = lnCellStart + 1;

        //<< Si sobrepasa las celdas regresamos.
        if (lnCellEnd >= loList.Count) return false;

        //<< Obtenemos la posicion normalizada entre celdas.
        float lnNormalizedDistanceBetwenCells = lnDeltaPath - lnCellStart;

        BoardCell<BoardUnit> loCellStart = loList[lnCellStart];
        BoardCell<BoardUnit> loCellEnd = loList[lnCellEnd];

        //<< Si estamos sobre el mismo eje en X sabemos que debemos movernos sobre Z.
        if (loCellStart.X == loCellEnd.X)
        {
            if (loCellStart.Z < loCellEnd.Z) loRotation = Forward; //<< Forward
            else loRotation = Backward; //<< Backward
        }
        else//<< Si estamos sobre el mismo eje Z sabemos que debemos movernos sobre X.
        {
            if (loCellStart.X < loCellEnd.X) loRotation = Right;//<< Right
            else loRotation = Left; //<< Left
        }

        //<< Determinamos si hay una curva y si esta es la primera parte de la curva.
        int lnCellNext = lnCellEnd + 1;
        if (lnCellNext < loList.Count && lnNormalizedDistanceBetwenCells > 0.5f)
        {
            BoardCell<BoardUnit> loCellNext = loList[lnCellNext];

            //<< Si es una curva comenzamos viaje por curva.
            if (loCellStart.X != loCellNext.X && loCellStart.Z != loCellNext.Z)
            {
                //<< Ya que la curva solo aplica hasta la mitad del desplazamiento.
                float lnNormalizedOffset = lnNormalizedDistanceBetwenCells - 0.5f;

                //<< Normalizamos el desplazamiento actual, como sabemos que solo nos desplazamos la mitad de una unidad.
                float lnNormalized = lnNormalizedOffset / 0.5f;

                //<< Calculamos la posicion que debe tener para realizar la curva.
                Vector3 loDirectionStart = loCellEnd.Vector - loCellNext.Vector;
                Vector3 loDirectionEnd = loCellEnd.Vector - loCellStart.Vector;

                Quaternion loRotationStart = Quaternion.LookRotation(loDirectionStart, Vector3.up);
                Quaternion loRotationEnd = Quaternion.LookRotation(loDirectionEnd, Vector3.up);

                //<< Obtenemos el punto medio ya que es la primera parte de la curva.
                loRotationEnd = Quaternion.Lerp(loRotationStart, loRotationEnd, 0.5f);

                //<< Rotamos con base en el delta normalizado.
                loPosition = Quaternion.Lerp(loRotationStart, loRotationEnd, lnNormalized) * (Vector3.forward * 0.5f);

                //<< Determinamos el pivote de rotación.
                float lnPivotX = (loCellNext.X - loCellStart.X) * 0.5f;
                float lnPivotZ = (loCellNext.Z - loCellStart.Z) * 0.5f;

                //<< Agregamos la posicion de la celda mas el pivote.
                loPosition.x += loCellStart.X + lnPivotX;
                loPosition.z += loCellStart.Z + lnPivotZ;

                //<< Si estamos sobre el mismo eje en X sabemos que debemos movernos sobre Z.
                if (loCellEnd.X == loCellNext.X)
                {
                    if (loCellEnd.Z < loCellNext.Z) loRotationEnd = Forward; //<< Forward
                    else loRotationEnd = Backward; //<< Backward
                }
                else//<< Si estamos sobre el mismo eje Z sabemos que debemos movernos sobre X.
                {
                    if (loCellEnd.X < loCellNext.X) loRotationEnd = Right;//<< Right
                    else loRotationEnd = Left; //<< Left
                }

                //<< Asignamos la rotacion que tendrá la unidad.
                loRotation = Quaternion.Lerp(loRotation, loRotationEnd, lnNormalizedOffset);

                return true;
            }
        }

        //<< Determinamos si hay una curva y si esta es la segunda parte de la curva.
        int lnCellPrior = lnCellStart - 1;
        if (lnCellPrior > -1 && lnNormalizedDistanceBetwenCells < 0.5f)
        {
            BoardCell<T> loCellPrior = loList[lnCellPrior];

            //<< Si es una curva comenzamos viaje por curva.
            if (loCellPrior.X != loCellEnd.X && loCellPrior.Z != loCellEnd.Z)
            {
                //<< Ya que la curva solo aplica hasta la mitad del desplazamiento.
                float lnNormalizedOffset = lnNormalizedDistanceBetwenCells + 0.5f;

                //<< Normalizamos el desplazamiento actual, como sabemos que solo nos desplazamos la mitad de una unidad.
                float lnNormalized = lnNormalizedDistanceBetwenCells / 0.5f;

                //<< Calculamos la posicion que debe tener para realizar la curva.
                Vector3 loDirectionStart = loCellStart.Vector - loCellEnd.Vector;
                Vector3 loDirectionEnd = loCellStart.Vector - loCellPrior.Vector;

                Quaternion loRotationStart = Quaternion.LookRotation(loDirectionStart, Vector3.up);
                Quaternion loRotationEnd = Quaternion.LookRotation(loDirectionEnd, Vector3.up);

                //<< Obtenemos el punto medio ya que es la primera parte de la curva.
                loRotationStart = Quaternion.Lerp(loRotationStart, loRotationEnd, 0.5f);

                //<< Rotamos con base en el delta normalizado.
                loPosition = Quaternion.Lerp(loRotationStart, loRotationEnd, lnNormalized) * (Vector3.forward * 0.5f);

                //<< Determinamos el pivote de rotación.
                float lnPivotX = (loCellEnd.X - loCellPrior.X) * 0.5f;
                float lnPivotZ = (loCellEnd.Z - loCellPrior.Z) * 0.5f;

                //<< Agregamos la posicion de la celda mas el pivote.
                loPosition.x += loCellPrior.X + lnPivotX;
                loPosition.z += loCellPrior.Z + lnPivotZ;

                //<< Si estamos sobre el mismo eje en X sabemos que debemos movernos sobre Z.
                if (loCellPrior.X == loCellStart.X)
                {
                    if (loCellPrior.Z < loCellStart.Z) loRotationStart = Forward; //<< Forward
                    else loRotationStart = Backward; //<< Backward
                }
                else//<< Si estamos sobre el mismo eje Z sabemos que debemos movernos sobre X.
                {
                    if (loCellPrior.X < loCellStart.X) loRotationStart = Right;//<< Right
                    else loRotationStart = Left; //<< Left
                }

                loRotation = Quaternion.Lerp(loRotationStart, loRotation, lnNormalizedOffset);

                return true;
            }
        }

        loPosition.x = Mathf.Lerp(loCellStart.X, loCellEnd.X, lnNormalizedDistanceBetwenCells);
        loPosition.z = Mathf.Lerp(loCellStart.Z, loCellEnd.Z, lnNormalizedDistanceBetwenCells);

        return true;
    }

    /// <summary>
    /// Se llama cuando la unidad seleccionada realiza un movimiento controlado.
    /// </summary>
    private void OnUnitAnimatorMoveManually(UnitInstance loUnitInstance)
    {
        loUnitInstance.transform.position += loUnitInstance.animator.deltaPosition;
    }

    /// <summary>
    /// Se llama para detener el movimiento de la unidad.
    /// </summary>
    /// <param name="loUnitInstance"></param>
    private void EndUnitInstanceMove(UnitInstance loUnitInstance)
    {
        loUnitInstance.animator.SetTrigger(AnimationControllerData.Triggers.EndMove);
        loUnitInstance.OnUnitAnimatorMove = null;
        gnDeltaCellPath = -1.0f;
    }

    /// <summary>
    /// Convierte un string a vector3.
    /// </summary>
    /// <param name="lcValue"></param>
    /// <returns></returns>
    private static bool TryParseToVector3(string lcValue, out Vector3 loResult)
    {
        loResult = Vector3.zero;

        //<< Validamos que esté bien estructurado el string.
        if (!lcValue.Contains('(') || !lcValue.Contains(')')) return false;
        if (lcValue.Count(x => x.Equals(',')) != 2 || lcValue.Length < 7) return false;

        string[] loTemp = lcValue.Substring(1, lcValue.Length - 2).Split(',');
        loResult = new Vector3(float.Parse(loTemp[0]), float.Parse(loTemp[1]), float.Parse(loTemp[2]));
        return true;
    }

    /// <summary>
    /// Se llama para crear el entramado del tablero de juego.
    /// </summary>
    /// <param name="lnWidht"></param>
    /// <param name="lnLenght"></param>
    private void CreateGridAndChecker(int lnWidht, int lnLenght)
    {
        //<< Creamos el grid con base en las dimensiones dadas.
        int lnPrefabLinesHashCode = gridLine.GetHashCode();
        int lnPrefabCheckersHashCode = checker.GetHashCode();

        //<< Desactivamos todo antes.
        DeactivateSpawns(lnPrefabLinesHashCode);
        goGridLinesBorder.Clear();
        goGridLines.Clear();
        goCheckers.Clear();

        //<< Creamos las lineas que van sobre el eje Z.
        int lnCount = lnWidht + 1;
        float lnZ = (lnLenght * 0.5f) - 0.5f;
        for (int x = 0; x < lnCount; x++)
        {
            Transform loTransform = Spawn(lnPrefabLinesHashCode, gridLine);
            loTransform.SetParent(boardParent);
            loTransform.position = new Vector3(x - 0.5f, goBoardProperties.gridLineOffsetY, lnZ);
            if (x == 0 || x >= lnWidht)
            {
                loTransform.localScale = new Vector3(goBoardProperties.borderLineWidth, 1.0f, lnLenght);
                goGridLinesBorder.Add(loTransform);
            }
            else
            {
                loTransform.localScale = new Vector3(goBoardProperties.gridLineWidth, 1.0f, lnLenght);
                goGridLines.Add(loTransform);
            }
        }

        //<< Creamos las lineas que van sobre el eje X.
        lnCount = lnLenght + 1;
        float lnX = (lnWidht * 0.5f) - 0.5f;
        for (int z = 0; z < lnCount; z++)
        {
            Transform loTransform = Spawn(lnPrefabLinesHashCode, gridLine);
            loTransform.SetParent(boardParent);
            loTransform.position = new Vector3(lnX, goBoardProperties.gridLineOffsetY, z - 0.5f);
            if (z == 0 || z >= lnLenght)
            {
                loTransform.localScale = new Vector3(lnWidht, 1.0f, goBoardProperties.borderLineWidth);
                goGridLinesBorder.Add(loTransform);
            }
            else
            {
                loTransform.localScale = new Vector3(lnWidht, 1.0f, goBoardProperties.gridLineWidth);
                goGridLines.Add(loTransform);
            }
        }

        //<< Creamos los checkers.
        bool lbChecker = false;
        for (int z = 0; z < lnLenght; z++)
        {
            lbChecker = !lbChecker;
            for (int x = 0; x < lnWidht; x++)
            {
                lbChecker = !lbChecker;

                if (lbChecker) continue;
                Transform loTransform = Spawn(lnPrefabCheckersHashCode, checker);
                loTransform.SetParent(boardParent);
                loTransform.name = "Checker";
                loTransform.position = new Vector3(x, goBoardProperties.gridLineOffsetY, z);
                goCheckers.Add(loTransform);
            }
        }

        //<< Si nos piden lineas de borde.
        goGridLinesBorder.SetActive(goBoardProperties.borderLines);
        goGridLines.SetActive(goBoardProperties.gridLines);
        goCheckers.SetActive(goBoardProperties.checker);
    }

    /// <summary>
    /// Se llama para crear las celdas de selección del tablero.
    /// </summary>
    /// <param name="lnWidth"></param>
    /// <param name="lnLenght"></param>
    private void CreateSelectionCells(int lnWidth, int lnLenght)
    {
        //<< Creamos las celdas de selección con base en las dimensiones dadas.
        int lnPrefabsHashCode = prefabBoardCellSelector.GetHashCode();

        //<< Desactivamos todo antes.
        DeactivateSpawns(lnPrefabsHashCode);

        //<< Creamos los selectores del tablero.
        for (int x = 0; x < lnWidth; x++)
        {
            for (int z = 0; z < lnLenght; z++)
            {
                Transform loTransform = Spawn(lnPrefabsHashCode, prefabBoardCellSelector);
                loTransform.SetParent(boardParent);
                loTransform.position = new Vector3(x, 0.0f, z);
                loTransform.name = goBoard.CellIndexOf(x, z).ToString();
            }
        }
    }

    /// <summary>
    /// Intentamos seleccionar una unidad de la celda seleccionada.
    /// </summary>
    private bool TrySelectUnit(int lnCellIndex, out BoardUnit loSelectedBoardUnit)
    {
        //<< Obtenemos la celda seleccionada.
        BoardCell<BoardUnit> loBoardCell = goBoard.GetCell(lnCellIndex);

        //<< Obtenemos la unidad de la celda seleccionada.
        loSelectedBoardUnit = loBoardCell.Data.FirstOrDefault(x => x.UnitProperties.UnitBattleProperties.UnitType == UnitType.Unit);

        //<< Si en la celda no hay unidad seleccionable posicionamos el elemento de celda vacia y regresamos.
        if (loSelectedBoardUnit == null)
        {
            //<< Mostramos la celda vacia.
            boardCellSelectedEmpty.gameObject.SetActive(true);
            boardCellSelectedEmpty.transform.position = new Vector3(loBoardCell.X, 0.0f, loBoardCell.Z);
            return false;
        }

        //<< Si es la celda de nuestra unidad ya seleccionada regresamos.
        if (loSelectedBoardUnit != null && (loSelectedBoardUnit == goSelectedBoardUnit || loSelectedBoardUnit == goSelectedBoardUnitSecondary)) return false;

        //<< Si no hay acciones no aparecen las celdas.
        if (goBoard.GetActionsCount(gnUserId) <= 0) return false;

        return true;
    }

    private void OnActionModeChanged()
    {
        ////<< Revisamos si es alguna de las celdas que rodean la unidad para alguna acción.
        //switch (goUnitActionMode)
        //{
        //    case UnitActionMode.Move:
        //        //<< Revisamos si es una de las celdas circundantes, si es así obtenemos el path mas acercado.
        //        if (goUnitSurroundingCells.Contains(loBoardCell))
        //        {
        //            //<< Desactivamos las celdas previamente seleccionadas.
        //            DeactivateCellsSelected();

        //            //<< Obtenemos cuantas celdas puede desplazarse la unidad.
        //            int lnDistance = loSelectedBoardUnit.GetMovement() + 1;

        //            //<< Obtenemos los indices de las celdas seleccionadas.
        //            int lnCellIndexStart = goBoard.CellIndexOf(loSelectedBoardUnit.X, loSelectedBoardUnit.Z);
        //            int lnCellIndexEnd = goBoard.CellIndexOf(loBoardCell.X, loBoardCell.Z);

        //            //<< Si la unidad se teletransporta solo podemos mostrar dos celdas, la de la unidad y la elegida.
        //            if (loSelectedBoardUnit.UnitBattleProperties.Teleports)
        //            {
        //                goUnitCellsSelected.Add(goBoard.GetCell(lnCellIndexStart));
        //                goUnitCellsSelected.Add(goBoard.GetCell(lnCellIndexEnd));
        //            }
        //            else
        //            {
        //                //<< Obtenemos el camino de celdas del inicio al fin.
        //                goBoard.GetClosestPath(lnCellIndexStart, lnCellIndexEnd, lnDistance, BoardCellPassThrough, goUnitCellsSelected);

        //                //<< Revertimos el path para que inicie desde la unidad.
        //                goUnitCellsSelected.Reverse();
        //            }

        //            //<< Obtenemos el hashcode de la celda que representa las celdas de desplazamiento.
        //            int lnHashCode = prefabBoardCellSelected.GetHashCode();

        //            //<< Instanciamos el rango de celdas por las que puede moverse la unidad.
        //            Transform loDisplacementCell;
        //            foreach (BoardCell<List<UnitProperties>> loItem in goUnitCellsSelected)
        //            {
        //                loDisplacementCell = Spawn(lnHashCode, prefabBoardCellSelected);
        //                loDisplacementCell.position = loItem.Vector;
        //            }

        //            //<< Como ya tenemos celdas seleccionadas solo regresamos.
        //            return;
        //        }
        //        break;
        //    case UnitActionMode.Attack: OnUnitActionModeAttack(loBoardCell, loSelectedBoardUnit); break;
        //    default: break;
        //}
    }

    /// <summary>
    /// Evento de OnClick en sobre el tablero para seleccionar u otras cosas.
    /// Nota: Esta funcion se llama en los eventos del panel de UICombat atraves de llamadas del GameCommand.
    /// </summary>
    private void OnBoardMouseDrag()
    {
        //<< Si nos indican que debemos desplazar la unidad seleccionada solo regresamos durante este proceso.
        if (gnDeltaCellPath > -1) return;

        //<< Si no mantenemos el boton presionado regresamos.
        if (!Mouse.current.leftButton.isPressed) return;

        Vector2 loMousePosition = Mouse.current.position.ReadValue();

        //<< Actualizamos solo si hay cambio de posición.
        if (gnMousePositionMagnitude == loMousePosition.magnitude) return;
        gnMousePositionMagnitude = loMousePosition.magnitude;

        //<< Si no hay celdas que podamos seleccionar regresamos.
        if (goUnitSurroundingCells.Count == 0) return;

        //<< Si no tenemos selección solo regresamos.
        if (goSelectedBoardUnit == null) return;

        //<< Si ya no tenemos acciones regresamos.
        switch (goUnitActionMode)
        {
            case UnitActionMode.Move: if (goSelectedBoardUnit.Moves <= 0) return; break;
            case UnitActionMode.Attack: if (goSelectedBoardUnit.Attacks <= 0) return; break;
            default: return;
        }

        //<< Obtenemos la celda seleccionada.
        goRay = Camera.main.ScreenPointToRay(loMousePosition);
        if (!Physics.Raycast(goRay, out goRaycastHit, Mathf.Infinity, Layer.BoardCellMask)) return;

        //<< Seleccionamos la celda. Nota: Sabemos que el nombre del GameObject es el id/index de la celda.
        int lnCellIndex = int.Parse(goRaycastHit.collider.gameObject.name);

        //<< Obtenemos la celda seleccionada.
        BoardCell<BoardUnit> loBoardCell;

        //<< Determinamos el modo de accion de la unidad seleccionada.
        switch (goUnitActionMode)
        {
            //<< Ya que es modo drag debemos ir agregando celdas a la lista de celdas del camino seleccionado.
            case UnitActionMode.Move:
                {
                    //<< Si es la misma celda donde está la unidad simplemente limpiamos y regresamos.
                    if (gnSelectedCellIndex == lnCellIndex)
                    {
                        goUnitCellsSelected.Clear();
                        DeactivateSpawns(prefabBoardCellSelected.GetHashCode());
                        return;
                    }

                    //<< Revisamos si la celda se encuentra dentro de las celdas aceptables.
                    loBoardCell = goUnitSurroundingCells.FirstOrDefault(x => x.Index == lnCellIndex);

                    //<< Si no se encontró solo regresamos.
                    if (loBoardCell == null) return;

                    //<< Solo si tenemos celdas seleccionadas revisamos...
                    if (goUnitCellsSelected.Count > 0)
                    {
                        //<< Revisamos si ya se encuentra dentro de la lista de seleccionados, de ser así regresamos.
                        int lnIndexOfBoardCell = goUnitCellsSelected.IndexOf(loBoardCell);

                        //<< Si se encuentra en la lista revisamos.
                        if (lnIndexOfBoardCell > -1)
                        {
                            //<< Si seguimos en la ultima celda regresamos.
                            if (lnIndexOfBoardCell == goUnitCellsSelected.Count - 1) return;

                            //<< Si no es la celda donde está la unidad liberamos las demás para regresar a la seleccionada.
                            lnIndexOfBoardCell++;
                            int lnCount = goUnitCellsSelected.Count - lnIndexOfBoardCell;
                            if (lnCount > 0) goUnitCellsSelected.RemoveRange(lnIndexOfBoardCell, lnCount);

                            //<< Removemos las instancias de las celdas del camino seleccionado.
                            DeactivateSpawns(prefabBoardCellSelected.GetHashCode());

                            //<< Creamos nuevas instancias de camino seleccionado.
                            Transform loCellSelectedIn;
                            int lnHashCodeIn = prefabBoardCellSelected.GetHashCode();
                            foreach (BoardCell<BoardUnit> loItem in goUnitCellsSelected)
                            {
                                //<< Instanciamos la celda seleccionada.
                                loCellSelectedIn = Spawn(lnHashCodeIn, prefabBoardCellSelected);
                                loCellSelectedIn.position = new Vector3(loItem.X, 0.0f, loItem.Z);
                            }

                            return;
                        }
                    }

                    //<< Revisamos si la celda no es vecina de la ultima celda seleccionada, de ser así creamos nuevo path.
                    if (goSelectedBoardUnit.GetMovement() < goUnitCellsSelected.Count || (goUnitCellsSelected.Count == 0 && !goBoard.GetCell(gnSelectedCellIndex).IsNeighbor(loBoardCell)) || (goUnitCellsSelected.Count > 0 && !goUnitCellsSelected[goUnitCellsSelected.Count - 1].IsNeighbor(loBoardCell)))
                    {
                        OnSelectMoveCells(lnCellIndex);
                        return;
                    }

                    //<< Agregamos la celda seleccionada ya que cumple con los requisitos.
                    goUnitCellsSelected.Add(loBoardCell);

                    //<< Obtenemos el hashcode de la celda que representa las celdas de desplazamiento.
                    int lnHashCode = prefabBoardCellSelected.GetHashCode();

                    //<< Instanciamos la celda seleccionada.
                    Transform loCellSelected = Spawn(lnHashCode, prefabBoardCellSelected);
                    loCellSelected.position = new Vector3(loBoardCell.X, 0.0f, loBoardCell.Z);
                }
                break;
            default: break;
        }
    }

    /// <summary>
    /// Evento de OnClick en sobre el tablero para seleccionar u otras cosas.
    /// Nota: Esta funcion se llama en los eventos del panel de UICombat atraves de llamadas del GameCommand.
    /// </summary>
    private void OnBoardMouseUp()
    {
        //<< Revisamos si se liberó el boton izquierdo en este frame.
        if (!Mouse.current.leftButton.wasReleasedThisFrame) return;

        //<< Si estamos en modo de confirmación regresamos por que no hay nada que hacer mas que confirmar.
        if (goUnitActionState == UnitActionState.ConfirmingAction) return;

        //<< Obtenemos posicion del mouse.
        Vector2 loMousePosition = Mouse.current.position.ReadValue();
        gnMousePositionMagnitude = loMousePosition.magnitude;

        //<< Desactivamos la celda vacia.
        boardCellSelectedEmpty.gameObject.SetActive(false);

        //<< Determinamos cual fue la celda seleccionada.
        goRay = Camera.main.ScreenPointToRay(loMousePosition);
        if (!Physics.Raycast(goRay, out goRaycastHit, Mathf.Infinity, Layer.BoardCellMask)) return;

        //<< Seleccionamos la celda. Nota: Sabemos que el nombre del GameObject es el index de la celda.
        int lnCellIndex = int.Parse(goRaycastHit.collider.gameObject.name);

        //<< Dependiendo del estado de acción...
        switch (goUnitActionState)
        {
            case UnitActionState.SelectingUnit:
                if (TrySelectUnit(lnCellIndex, out BoardUnit loSelectedBoardUnit))
                {
                    //<< Avisamos de nueva selección.
                    if (loSelectedBoardUnit.UserId == gnUserId)
                    {
                        gnSelectedCellIndex = lnCellIndex;
                        goSelectedBoardUnit = loSelectedBoardUnit;
                        Send(GameCommand.SetSelectedUnit, gnUserId, loSelectedBoardUnit, unitOutlineColorUser);

                        //<< Creamos la unidad del portaretrato.
                        if (goUnitInstancePortraitUser == null || goUnitInstancePortraitUser.UnitProperties.Id != loSelectedBoardUnit.UnitProperties.Id) CreatePortraitUnit(loSelectedBoardUnit.UnitProperties, goBoard.UserIdA == gnUserId ? goBoard.FactionUserA : goBoard.FactionUserB, cameraUnitPortraitPrimary, ref goUnitInstancePortraitUser, Layer.UnitPortraitPrimary);

                        //<< Si es nuestro turno mostramos la interfaz de acciones de unidad.
                        if (goBoard.Turn == gnUserId)
                        {
                            //<< Le avisamos al UI de combate que muestre las acciones.
                            Send(GameCommand.UICombat_DisplayActions, true);

                            //<< Le indicamos que botones debe habilitar o no al UI de combate.
                            Send(GameCommand.UICombat_EnableActionAttack, goSelectedBoardUnit.Attacks > 0);
                            Send(GameCommand.UICombat_EnableActionMove, goSelectedBoardUnit.Moves > 0);

                            //<< TODO: Falta revisar como saber si tiene acción de curación.
                            //Send(GameCommand.UICombat_DisplayActionHeal, goSelectedBoardUnit.Attacks > 0);
                            //Send(GameCommand.UICombat_EnableActionHeal, goSelectedBoardUnit.Attacks > 0);
                        }
                    }
                    else
                    {
                        goSelectedBoardUnitSecondary = loSelectedBoardUnit;
                        Send(GameCommand.SetSelectedUnit, gnUserId, loSelectedBoardUnit, unitOutlineColorSecundary);

                        //<< Creamos la unidad del portaretrato.
                        if (goUnitInstancePortraitSecondary == null || goUnitInstancePortraitSecondary.UnitProperties.Id != loSelectedBoardUnit.UnitProperties.Id) CreatePortraitUnit(loSelectedBoardUnit.UnitProperties, goBoard.UserIdA == gnUserId ? goBoard.FactionUserA : goBoard.FactionUserB, cameraUnitPortraitSecondary, ref goUnitInstancePortraitSecondary, Layer.UnitPortraitSecondary);
                    }
                }
                break;
            case UnitActionState.SelectingCells:
                //<< Si la celda no está dentro del area de seleccion solo regresamos.
                if (!goUnitSurroundingCells.Any(x => x.Index == lnCellIndex)) return;

                //<< Obtenemos la celda seleccionada.
                BoardCell<BoardUnit> loBoardCellSelected = goBoard.GetCell(lnCellIndex);

                //<< Desactivamos las celdas previamente seleccionadas.
                DeactivateCellsSelected();

                //<< Obtenemos el hashcode de la celda que representa la celda de selección.
                int lnHashCode = prefabBoardCellSelected.GetHashCode();

                //<< Revisamos si es alguna de las celdas que rodean la unidad para alguna acción.
                switch (goUnitActionMode)
                {
                    case UnitActionMode.Move:
                        OnSelectMoveCells(lnCellIndex);

                        //<< Si solo es una celda sabemos que es la misma celda en la que está la unidad por ello debemos detener el flujo.
                        if (goUnitCellsSelected.Count == 1)
                        {
                            DeactivateCellsSelected();
                            goUnitCellsSelected.Clear();
                        }

                        //<< Avisamos que debemos habilitar o deshabilitar el botón de confirmación.
                        Send(GameCommand.UICombat_DisplayConfirmation, goUnitCellsSelected.Count > 1, goUnitActionMode);
                        break;
                    case UnitActionMode.Attack:

                        //<< Agregamos la celda seleccionada ya que cumple con los requisitos.
                        goUnitCellsSelected.Add(loBoardCellSelected);

                        //<< Instanciamos la celda seleccionada.
                        Transform loCellSelected = Spawn(lnHashCode, prefabBoardCellSelected);
                        loCellSelected.position = new Vector3(loBoardCellSelected.X, 0.0f, loBoardCellSelected.Z);

                        //<< Mostramos el area de efecto.
                        CreateEffectAreaOf(goSelectedBoardUnit, loBoardCellSelected, goSelectedBoardUnit.GameplayEffectsAttack);

                        //<< Si es una unidad con rango cero sabemos que debemos agregar la misma celda en la que se encuentra y seleccionar el area.
                        if (goSelectedBoardUnit.GetAttackRange() == 0) OnUnitZeroRange(goSelectedBoardUnit);
                        //<< Si no hay celdas desactivamos todo.
                        else if (goUnitCellsSelected.Count == 0) DeactivateCellsSelected();

                        //<< Revisamos si dentro de la celda seleccionada de ataque hay una unidad para mostrarla en el portaretrato.
                        if (goUnitCellsSelected.Count > 0)
                        {
                            BoardUnit loBoardUnitSelected = goUnitCellsSelected[goUnitCellsSelected.Count - 1].Data.FirstOrDefault(x => x.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit));
                            if (loBoardUnitSelected != null)
                            {
                                goSelectedBoardUnitSecondary = loBoardUnitSelected;

                                Send(GameCommand.SetSelectedUnit, gnUserId, goSelectedBoardUnitSecondary, unitOutlineColorSecundary);

                                //<< Creamos la unidad del portaretrato.
                                if (goUnitInstancePortraitSecondary == null || goUnitInstancePortraitSecondary.UnitProperties.Id != goSelectedBoardUnitSecondary.UnitProperties.Id) CreatePortraitUnit(goSelectedBoardUnitSecondary.UnitProperties, goBoard.UserIdA == gnUserId ? goBoard.FactionUserA : goBoard.FactionUserB, cameraUnitPortraitSecondary, ref goUnitInstancePortraitSecondary, Layer.UnitPortraitSecondary);
                            }
                            else GameManager.Send(GameCommand.UICombat_DisplayPortraitSecondary, false);
                        }

                        //<< Avisamos que debemos habilitar o deshabilitar el botón de confirmación.
                        GameManager.Send(GameCommand.UICombat_DisplayConfirmation, goUnitCellsSelected.Count > 0, goUnitActionMode);
                        break;
                    default: break;
                }
                break;
            default: break;
        }
    }

    private void OnSelectMoveCells(int lnCellIndexEnd)
    {
        //<< Obtenemos cuantas celdas puede desplazarse la unidad.
        int lnDistance = goSelectedBoardUnit.GetMovement() + 1;

        //<< Obtenemos los indices de las celdas seleccionadas.
        int lnCellIndexStart = gnSelectedCellIndex;

        //<< Si la unidad se teletransporta solo podemos mostrar dos celdas, la de la unidad y la elegida.
        if (goSelectedBoardUnit.UnitProperties.UnitBattleProperties.Teleports)
        {
            goUnitCellsSelected.Add(goBoard.GetCell(lnCellIndexStart));
            goUnitCellsSelected.Add(goBoard.GetCell(lnCellIndexEnd));
        }
        else
        {
            //<< Obtenemos el camino de celdas del inicio al fin.
            goBoard.GetClosestPath(lnCellIndexStart, lnCellIndexEnd, lnDistance, BoardCellPassThrough, goUnitCellsSelected);

            //<< Removemos la ultima celda que es la de la unidad para que no esté seleccionada.
            if (goUnitCellsSelected.Count > 0) goUnitCellsSelected.RemoveAt(goUnitCellsSelected.Count - 1);

            //<< Revertimos el path para que inicie desde la unidad.
            goUnitCellsSelected.Reverse();
        }

        //<< Instanciamos el rango de celdas por las que puede moverse la unidad.
        Transform loDisplacementCell;
        int lnHashCode = prefabBoardCellSelected.GetHashCode();
        foreach (BoardCell<BoardUnit> loItem in goUnitCellsSelected)
        {
            loDisplacementCell = Spawn(lnHashCode, prefabBoardCellSelected);
            loDisplacementCell.position = new Vector3(loItem.X, 0.0f, loItem.Z);
        }
    }

    /// <summary>
    /// Se llama para las unidades con rango cero.
    /// </summary>
    private void OnUnitZeroRange(BoardUnit loBoardUnit)
    {
        DeactivateCellsSelected();
        if (loBoardUnit.Attacks <= 0 || !goBoard.TryGetCell(loBoardUnit, out BoardCell<BoardUnit> loBoardCell)) return;
        goUnitSurroundingCells.Add(loBoardCell);
    }

    /// <summary>
    /// Limpia el las celdas seleccionadas.
    /// </summary>
    private void DeactivateCellsSelected()
    {
        //<< Liberamos las celdas seleccionadas.
        goUnitCellsSelected.Clear();

        //<< Removemos las instancias de las celdas del camino seleccionado.
        DeactivateSpawns(prefabBoardCellSelected.GetHashCode());
    }

    /// <summary>
    /// Desactiva nuestra seleccion de celdas previas.
    /// </summary>
    private void DeactivateBoardCellsSelections()
    {
        //<< Liberamos cualquier referencia previa.
        goUnitSurroundingCells.Clear();
        goCellsEffectArea.Clear();

        boardCellSelectedEmpty.gameObject.SetActive(false);

        //<< Desactivamos cualquier celda previa de desplazamiento.
        DeactivateSpawns(prefabBoardCellMove.GetHashCode());
        DeactivateSpawns(prefabBoardCellAttack.GetHashCode());
        DeactivateSpawns(prefabBoardCellEffectArea.GetHashCode());

        //<< Desactivamos el path seleccionado.
        DeactivateCellsSelected();
    }

    /// <summary>
    /// Crea las celdas alrededor con base en la distancia.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lnDistance"></param>
    /// <param name="lnCellX"></param>
    /// <param name="lnCellZ"></param>
    /// <param name="loPrefabBoardCell"></param>
    /// <returns></returns>
    private static void CreateSurroundingCells<T>(T loPrefabBoardCell, Func<BoardCell<BoardUnit>, Color> loGetcolor) where T : Component
    {
        //<< Si no hay celdas solo regresamos.
        if (goUnitSurroundingCells.Count <= 0) return;

        //<< Obtenemos el hashcode del prefab que representa las celdas.
        int lnHashCode = loPrefabBoardCell.GetHashCode();

        //<< Instanciamos el rango de celdas.
        T loPrefabCell;
        BoardCell<BoardUnit> loBoardCell;
        int lnCount = goUnitSurroundingCells.Count;
        for (int i = 0; i < lnCount; i++)
        {
            loBoardCell = goUnitSurroundingCells[i];
            loPrefabCell = Spawn(lnHashCode, loPrefabBoardCell);
            loPrefabCell.transform.position = new Vector3(loBoardCell.X, 0.0f, loBoardCell.Z);
            SetBoardElementColor(loPrefabCell, loGetcolor(loBoardCell));
        }
    }

    /// <summary>
    /// Crea el efecto en area con base en los datos de la unidad.
    /// </summary>
    /// <param name="loBoardUnit"></param>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="loUnitEventTiming"></param>
    private void CreateEffectAreaOf(BoardUnit loBoardUnit, BoardCell<BoardUnit> loBoardCellTarget, List<GameplayEffect> loGameplayEffects)
    {
        //<< Desactivamos las celdas para mostrar el area.
        DeactivateCellsEffectArea();

        //<< Si no hay efectos solo regresamos.
        if (loGameplayEffects.Count == 0) return;

        //<< Determinamos la direccion en la que irá el ataque.
        goBoard.TryGetCell(loBoardUnit, out BoardCell<BoardUnit> loBoardCellOrigin);
        Facing loFacing = FacingExtensions.GetFacing(loBoardCellOrigin.X, loBoardCellOrigin.Z, loBoardCellTarget.X, loBoardCellTarget.Z, loBoardUnit.Facing);

        //<< Obtenemos el area que afectaremos.
        foreach (GameplayEffect loGameplayEffect in loGameplayEffects) GetGameplayEffectArea(loBoardUnit, loGameplayEffect, loBoardCellTarget.Index, loFacing, goCellsEffectArea);

        //<< Obtenemos el hashcode de la celda que representa las celdas del area afectada.
        int lnHashCode = prefabBoardCellEffectArea.GetHashCode();

        //<< Instanciamos las celdas para mostrar el area del efecto.
        Transform loEffectAreaCell;
        foreach (BoardCell<BoardUnit> loItem in goCellsEffectArea)
        {
            loEffectAreaCell = Spawn(lnHashCode, prefabBoardCellEffectArea);
            loEffectAreaCell.position = new Vector3(loItem.X, 0.0f, loItem.Z);
        }
    }

    /// <summary>
    /// Limpia las celdas del area de efecto.
    /// </summary>
    private void DeactivateCellsEffectArea()
    {
        //<< Liberamos las celdas del area.
        goCellsEffectArea.Clear();

        //<< Removemos las instancias de las celdas del area de efecto.
        DeactivateSpawns(prefabBoardCellEffectArea.GetHashCode());
    }

    /// <summary>
    /// Nos da el area de la coleccion de efectos a ser usados.
    /// </summary>
    /// <param name="lnGameplayEffectCollectionId"></param>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="loFacing"></param>
    /// <returns></returns>
    private void GetGameplayEffectArea(BoardUnit loBoardUnitCaller, GameplayEffect loGameplayEffect, int lnCellIndex, Facing loFacing, List<BoardCell<BoardUnit>> loList)
    {
        //<< Dependiendo del area de cada efecto lo agregamos a la lista, pero revisamos para no mandar celdas repetidas.
        BoardCell<BoardUnit>[] loAreaCells = GetGameplayEffectAreaCells(loBoardUnitCaller, loGameplayEffect, lnCellIndex, loFacing, out int lnCells, false);

        if (lnCells == 0) return;
        if (loAreaCells == null) return;

        //<< Agregamos las celdas obtenidas del area.
        BoardCell<BoardUnit> loCell;
        for (int i = 0; i < lnCells; i++)
        {
            loCell = loAreaCells[i];
            if (!loList.Contains(loCell)) loList.Add(loCell);
        }
    }

    /// <summary>
    /// Nos da el area que afecta el efecto.
    /// </summary>
    /// <returns></returns>
    private BoardCell<BoardUnit>[] GetGameplayEffectAreaCells(BoardUnit loBoardUnitCaller, GameplayEffect loGameplayEffect, int lnCellIndex, Facing loFacing, out int lnCells, bool lbFiler = true)
    {
        //<< Aplicamos los efectos.
        lnCells = 0;
        BoardCell<BoardUnit>[] loAreaCells = null;

        if (loGameplayEffect.AreaType == GameplayEffectAreaType.None) return loAreaCells;

        //<< Obtenemos las celdas que afecta nuestro efecto.
        switch (loGameplayEffect.AreaType)
        {
            case GameplayEffectAreaType.Board: loAreaCells = goBoard.GetAreaBoard(out lnCells); break;
            case GameplayEffectAreaType.Line: loAreaCells = goBoard.GetAreaLineal(lnCellIndex, loGameplayEffect.Area, out lnCells, loFacing); break;
            case GameplayEffectAreaType.Radial: loAreaCells = goBoard.GetAreaRadial(lnCellIndex, loGameplayEffect.Area, out lnCells); break;
            case GameplayEffectAreaType.Cross: loAreaCells = goBoard.GetAreaCross(lnCellIndex, loGameplayEffect.Area, out lnCells); break;
            case GameplayEffectAreaType.X: loAreaCells = goBoard.GetAreaX(lnCellIndex, loGameplayEffect.Area, out lnCells); break;
            case GameplayEffectAreaType.Perpendicular: loAreaCells = goBoard.GetAreaPerpendicularCentered(lnCellIndex, loGameplayEffect.Area, out lnCells, loFacing); break;

            case GameplayEffectAreaType.T_Normal: loAreaCells = goBoard.GetAreaTNormal(lnCellIndex, loGameplayEffect.Area, out lnCells, loFacing); break;
            case GameplayEffectAreaType.T_Inverted: loAreaCells = goBoard.GetAreaTInverted(lnCellIndex, loGameplayEffect.Area, out lnCells, loFacing); break;

            case GameplayEffectAreaType.T_Normal_Hollow: loAreaCells = goBoard.GetAreaTNormalHollow(lnCellIndex, loGameplayEffect.Area, out lnCells, loFacing); break;
            case GameplayEffectAreaType.T_Inverted_Hollow: loAreaCells = goBoard.GetAreaTInvertedHollow(lnCellIndex, loGameplayEffect.Area, out lnCells, loFacing); break;

            case GameplayEffectAreaType.CrossHollow: loAreaCells = goBoard.GetAreaCrossHollow(lnCellIndex, loGameplayEffect.Area, out lnCells); break;
            case GameplayEffectAreaType.XHollow: loAreaCells = goBoard.GetAreaXHollow(lnCellIndex, loGameplayEffect.Area, out lnCells); break;

            case GameplayEffectAreaType.Chain: loAreaCells = goBoard.GetAreaChain(lnCellIndex, loGameplayEffect.Area, BoardCellChained, out lnCells); break;
            case GameplayEffectAreaType.Single: loAreaCells = goBoard.GetAreaSingle(lnCellIndex, out lnCells); break;
            case GameplayEffectAreaType.Self:
                {
                    lnCells = 1;
                    goBoard.TryGetCell(loBoardUnitCaller, out BoardCell<BoardUnit> loBoardCellCaller);
                    loAreaCells = goBoard.GetCellAsArray(loBoardCellCaller.Index);
                }
                break;

            case GameplayEffectAreaType.RandomPositionCross:
                lnCellIndex = goBoard.GetRandomCellIndex();
                loAreaCells = goBoard.GetAreaCross(lnCellIndex, loGameplayEffect.Area, out lnCells);
                break;

            //<< Sacamos la unidad mas lejana.
            case GameplayEffectAreaType.FurthestUnit:
                loAreaCells = goBoard.FurthestCell(CellContainsTarget, loBoardUnitCaller, loGameplayEffect.Target, loGameplayEffect.Area, lnCellIndex, out lnCells);
                break;

            //<< Sacamos la unidad mas cercana.
            case GameplayEffectAreaType.NearestUnit:
                loAreaCells = goBoard.NearestCell(CellContainsTarget, loBoardUnitCaller, loGameplayEffect.Target, loGameplayEffect.Area, lnCellIndex, out lnCells);
                break;

            case GameplayEffectAreaType.RandomEmptyPosition:
                {
                    ////<< FINDME: Solo se usa para pruebas.
                    //loAreaCells = goBoard.GetAreaBoard(out lnCells);
                    //int lnX = loUnitPropertiesCaller.X;
                    //int lnZ = loUnitPropertiesCaller.Z;
                    //switch (loUnitPropertiesCaller.Facing)
                    //{
                    //    case Facing.ZPos: lnZ++; break;
                    //    case Facing.ZNeg: lnZ--; break;
                    //    case Facing.XPos: lnX++; break;
                    //    case Facing.XNeg: lnX--; break;
                    //    default: break;
                    //}

                    //int lnCellIndexIn = goBoard.CellIndexOf(lnX, lnZ);
                    //lnCells = 1;
                    //loAreaCells = new BoardCell<List<UnitProperties>>[1] { goBoard.GetCell(lnCellIndexIn) };
                    ////<< FINDME: Solo se usa para pruebas.

                    loAreaCells = goBoard.GetAreaBoard(out lnCells);
                    Board<BoardUnit>.FilterCells(loAreaCells, EmptyCellsOnly, 0, 0, ref lnCells);
                    Board<BoardUnit>.GetRandomCell(loAreaCells, ref lnCells);
                }
                break;

            case GameplayEffectAreaType.RandomArroundTargetOrCaller:
                {
                    //<< Obtenemos todas las celdas en cruz del objetivo sin incluir el centro.
                    loAreaCells = goBoard.GetAreaCrossHollow(lnCellIndex, loGameplayEffect.Area, out lnCells);

                    //<< Filtramos con base en los requerimientos antes de pasar a obtener las celdas de quien llama el efecto.
                    if (lnCells > 0 && loGameplayEffect.Target != GameplayTarget.None) Board<BoardUnit>.FilterCells(loAreaCells, CellContainsTarget, loBoardUnitCaller, loGameplayEffect.Target, ref lnCells);

                    //<< Si no hay celdas para hacer el random pasamos a seleccionar una de las celdas circundantes de quien llama el efecto.
                    if (lnCells == 0)
                    {
                        goBoard.TryGetCell(loBoardUnitCaller, out BoardCell<BoardUnit> loBoardCellCaller);
                        loAreaCells = goBoard.GetAreaCrossHollow(loBoardCellCaller.Index, loGameplayEffect.Area, out lnCells);

                        //<< Filtramos con base en los requerimientos.
                        if (lnCells > 0 && loGameplayEffect.Target != GameplayTarget.None) Board<BoardUnit>.FilterCells(loAreaCells, CellContainsTarget, loBoardUnitCaller, loGameplayEffect.Target, ref lnCells);
                    }

                    //<< De las celdas resultantes elegimos una aleatoriamente.
                    Board<BoardUnit>.GetRandomCell(loAreaCells, ref lnCells);
                }
                break;

            default: break;
        }

        //<< Obtenemos las celdas con base en el tipo de objetivo.
        if (lbFiler && lnCells > 0 && loGameplayEffect.Target != GameplayTarget.None) Board<BoardUnit>.FilterCells(loAreaCells, CellContainsTarget, loBoardUnitCaller, loGameplayEffect.Target, ref lnCells);

        return loAreaCells;
    }

    /// <summary>
    /// Se llama para determinar si la celda tiene algún objetivo.
    /// </summary>
    /// <param name="loCell"></param>
    /// <param name="lnA"></param>
    /// <param name="lnB"></param>
    /// <returns></returns>
    private bool EmptyCellsOnly(BoardCell<BoardUnit> loCell, int lnA, int lnB)
    {
        //<< Si no hay celda no es target.
        if (loCell == null) return false;

        //<< Si no tiene unidad sabemos que si es viable.
        return loCell.Data.Count == 0;
    }

    /// <summary>
    /// Nos indica si la celda hace conexión con otras celdas.
    /// </summary>
    /// <param name="loBoardCell"></param>
    /// <returns></returns>
    private bool BoardCellChained(BoardCell<BoardUnit> loBoardCell)
    {
        //<< Si no tiene datos indicamos que no se conecta.
        if (loBoardCell.Data.Count == 0) return false;

        //<< Revisamos las unidades que se encuentran dentro de la celda.
        foreach (BoardUnit loBoardUnit in loBoardCell.Data)
        {
            //<< Ya que la unidad es creada por el juego "GameplayUnit" debemos revisar si es tipo unidad.
            if (loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)) return true;
        }

        //<< Indicamos que no hace conexión.
        return false;
    }

    /// <summary>
    /// Se llama para determinar si la celda tiene algún objetivo.
    /// </summary>
    /// <param name="loCell"></param>
    /// <param name="loBoardUnitCaller"></param>
    /// <param name="loGameplayEffectTarget"></param>
    /// <returns></returns>
    private bool CellContainsTarget(BoardCell<BoardUnit> loCell, BoardUnit loBoardUnitCaller, GameplayTarget loTarget)
    {
        //<< Si no hay celda no es target.
        if (loCell == null) return false;

        //<< Si es casilla vacia.
        if (loCell.Data.Count == 0 && loTarget.HasFlag(GameplayTarget.EmptyCell)) return true;

        //<< Si no tiene unidad regresamos por que de aqui en adelante revisamos si son unidades de algún tipo.
        if (loCell.Data.Count == 0) return false;

        //<< Revisamos que si se tengan objetivos en la lista.
        foreach (BoardUnit loBoardUnit in loCell.Data)
        {
            if (IsTarget(loBoardUnit, loBoardUnitCaller, loTarget)) return true;
        }

        return false;
    }

    /// <summary>
    /// Nos indica si la unidad es objetivo o no.
    /// </summary>
    /// <param name="loBoardUnit"></param>
    /// <param name="lbOpponent"></param>
    /// <param name="loTarget"></param>
    /// <returns></returns>
    private bool IsTarget(BoardUnit loBoardUnit, BoardUnit loBoardUnitCaller, GameplayTarget loTarget)
    {
        //<< Debe aplicarse solo a nuestra unidad.
        if (loTarget.HasFlag(GameplayTarget.SelfOnly)) return loBoardUnit == loBoardUnitCaller;

        //<< No debe aplicarse a nuestra unidad.
        if (loTarget.HasFlag(GameplayTarget.NotSelf) && loBoardUnit == loBoardUnitCaller) return false;

        //<< Unidades no.
        if (loTarget.HasFlag(GameplayTarget.NotUnit) && loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)) return false;

        //<< Unidades.
        if (loTarget.HasFlag(GameplayTarget.Unit) && !loTarget.HasFlag(GameplayTarget.GameplayUnit) && !loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)) return false;

        //<< Unidades del juego.
        if (loTarget.HasFlag(GameplayTarget.GameplayUnit) && !loTarget.HasFlag(GameplayTarget.Unit)) return false;

        //<< Unidades aliadas.
        if (loTarget.HasFlag(GameplayTarget.Ally) && loBoardUnit.UserId != loBoardUnitCaller.UserId) return false;

        //<< Unidades del oponente.
        if (loTarget.HasFlag(GameplayTarget.Opponent) && loBoardUnit.UserId == loBoardUnitCaller.UserId) return false;

        //<< Solo trofeos.
        if (loTarget.HasFlag(GameplayTarget.TrophyOnly) && !loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Trophy)) return false;

        //<< Solo tesoros.
        if (loTarget.HasFlag(GameplayTarget.TreasureOnly) && !loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Treasure)) return false;

        //<< Si nos indica que debe ser cazador y no es cazador indicamos que no es target.
        if (loTarget.HasFlag(GameplayTarget.HunterOnly) && loBoardUnit.UnitProperties.UnitBattleProperties.Role != UnitRole.Hunter) return false;

        //<< Si nos indica que debe ser rastreador y no es cazador indicamos que no es target.
        if (loTarget.HasFlag(GameplayTarget.TrackerOnly) && loBoardUnit.UnitProperties.UnitBattleProperties.Role != UnitRole.Tracker) return false;

        //<< Solo trampas.
        if (loTarget.HasFlag(GameplayTarget.TrapsOnly) && !loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Trap)) return false;

        //<< Solo unidades que no sean del Gameplay aka GameplayUnits.
        if (loTarget.HasFlag(GameplayTarget.UnitsOnly) && loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)) return false;

        return true;
    }

    /// <summary>
    /// Se usa para crear launidad que va en el portaretrato.
    /// </summary>
    /// <param name="loUnitInstance"></param>
    /// <param name="loCameraPortrait"></param>
    /// <param name="loUnitInstancePortrait"></param>
    private void CreatePortraitUnit(UnitProperties loUnitProperties, UnitFaction loUnitFaction, GameObject loCameraPortrait, ref UnitInstance loUnitInstancePortrait, int lnLayer)
    {
        //<< Si ya tenemos seleccionada e instanciada la unidad para el portraretrato solo regresamos.
        if (loUnitInstancePortrait != null && loUnitInstancePortrait.UnitProperties.Id == loUnitProperties.Id) return;

        //<< Desactivamos la unidad usada para el portaretrato y creamos nueva instancia.
        if (loUnitInstancePortrait != null) loUnitInstancePortrait.gameObject.SetActive(false);

        //<< Creamos un clon de la unidad para mostrarlo en el portaretrato.
        loUnitInstancePortrait = CreateUnitInstance(loUnitProperties, loUnitFaction);

        //<< Establecemos el layer de la unidad.
        loUnitInstancePortrait.SetLayer(lnLayer);

        //<< Ponemos a la unidad en la posicion donde está la camara.
        loUnitInstancePortrait.transform.position = Vector3.zero;
        loUnitInstancePortrait.transform.rotation = Quaternion.identity;

        //<< Activamos la camara de portraretrato.
        loCameraPortrait.SetActive(true);

        //<< Establecemos la posicion de la camara con respecto a la cabeza de la unidad.
        Vector3 loCameraPosition = loCameraPortrait.transform.position;
        loCameraPosition.y = loUnitInstancePortrait.headAttach.transform.position.y - 0.1f;
        loCameraPortrait.transform.position = loCameraPosition;
    }

    /// <summary>
    /// Funcion que nos sirve para validar si podemos atravesar la celda.
    /// </summary>
    /// <param name="loBoardCell"></param>
    /// <returns></returns>
    private bool BoardCellPassThrough(BoardCell<BoardUnit> loBoardCell)
    {
        //<< FINDME
        //<< TODO: Si la celda se encuentra en las celdas de la mascara sabemos que no podemos pasar por esta celda.

        //<< Revisamos las unidades que se encuentran dentro de la celda.
        foreach (BoardUnit loBoardUnit in loBoardCell.Data)
        {
            //<< Revisamos si la unidad es "GameplayUnit" y si es de tipo unidad sabemos que no podemos dejar pasar.
            if (loBoardUnit.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit)) return false;
        }

        //<< Indicamos si se puede pasar por esta celda.
        return true;
    }

    /// <summary>
    /// Se llama cuando se establece el modo de acción de la unidad, por ende debemos seleccionar las celdas para la acción.
    /// </summary>
    /// <param name="loUnitActionMode"></param>
    private void OnSetActionMode(UnitActionMode loUnitActionMode)
    {
        goUnitActionMode = loUnitActionMode;
        goUnitActionState = UnitActionState.SelectingCells;

        //<< Le avisamos al UI de combate que oculte las acciones ya que tenemos una seleccionada.
        Send(GameCommand.UICombat_DisplayActions, false);

        //<< Le avisamos al UI de combate que muestre el botón de regresar acción.
        Send(GameCommand.UICombat_DisplayReturnState, true);

        //<< Desactivamos cualquier celda de selección previa.
        DeactivateBoardCellsSelections();

        //<< Activamos las celdas de la acción.
        switch (goUnitActionMode)
        {
            case UnitActionMode.Move:
                {
                    //<< Obtenemos las celdas por las que puede moverse la unidad.
                    goBoard.GetPaths(gnSelectedCellIndex, goSelectedBoardUnit.GetMovement() + 1, BoardCellPassThrough, goUnitSurroundingCells);

                    //<< Creamos las celdas usando el rango de movimiento.
                    CreateSurroundingCells(prefabBoardCellMove, SurroundingCellsMoveValidation);

                    //<< Revisamos que celdas son validas para el movimiento con base en la unidad seleccionada.
                    int lnCount = goUnitSurroundingCells.Count;
                    for (int i = lnCount - 1; i >= 0; i--)
                    {
                        if (!ValidateMoveCell(goUnitSurroundingCells[i])) goUnitSurroundingCells.RemoveAt(i);
                    }
                }
                break;
            case UnitActionMode.Attack:
                {
                    int lnAttackRange = goSelectedBoardUnit.GetAttackRange() + 1;

                    //<< Obtenemos las celdas a las que puede atacar la unidad.
                    switch (goSelectedBoardUnit.UnitProperties.UnitBattleProperties.AttackAreaType)
                    {
                        case AttackAreaType.Cross: goBoard.GetAreaCross(gnSelectedCellIndex, lnAttackRange, goUnitSurroundingCells); break;
                        case AttackAreaType.X: goBoard.GetAreaX(gnSelectedCellIndex, lnAttackRange, goUnitSurroundingCells); break;
                        default: goBoard.GetAreaRadial(gnSelectedCellIndex, lnAttackRange, goUnitSurroundingCells); break;
                    }

                    //<< Creamos las celdas usando el rango de ataque.
                    CreateSurroundingCells(prefabBoardCellAttack, SurroundingCellsAttackValidation);

                    //<< Revisamos que celdas son validas para el ataque con base en la unidad seleccionada.
                    int lnCount = goUnitSurroundingCells.Count;
                    for (int i = lnCount - 1; i >= 0; i--)
                    {
                        if (!ValidateAttackCell(goUnitSurroundingCells[i])) goUnitSurroundingCells.RemoveAt(i);
                    }
                }
                break;
            default: break;
        }
    }

    /// <summary>
    /// Valida si la celda es valida para atacar.
    /// </summary>
    /// <param name="loCell"></param>
    /// <returns></returns>
    private bool ValidateAttackCell(BoardCell<BoardUnit> loCell)
    {
        if (goSelectedBoardUnit.UnitProperties.UnitBattleProperties.AllowAttackEmptySpace) return true;

        //<< Si no hay nadie en la celda...
        if (loCell.Data.Count == 0 || loCell.Data.Any(x => x.UserId == gnUserId && x.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit))) return false;

        return true;
    }

    /// <summary>
    /// Valida si la celda es valida para moverse en ella.
    /// </summary>
    /// <param name="loCell"></param>
    /// <returns></returns>
    private bool ValidateMoveCell(BoardCell<BoardUnit> loCell)
    {
        //<< Si no hay unidad en la celda...
        return loCell.Data.Count == 0 || loCell.Data.Any(x => !x.UnitProperties.UnitBattleProperties.UnitType.HasFlag(UnitType.Unit));
    }

    /// <summary>
    /// Valida si la celda es valida para atacar y regresa el color respectivo.
    /// </summary>
    /// <param name="loCell"></param>
    /// <returns></returns>
    private Color SurroundingCellsAttackValidation(BoardCell<BoardUnit> loCell)
    {
        return ValidateAttackCell(loCell) ? Color.red : Color.gray;
    }

    /// <summary>
    /// Valida si la celda es valida para moverse en ella y regresa el color respectivo.
    /// </summary>
    /// <param name="loCell"></param>
    /// <returns></returns>
    private Color SurroundingCellsMoveValidation(BoardCell<BoardUnit> loCell)
    {
        return ValidateMoveCell(loCell) ? Color.green : Color.gray;
    }

    /// <summary>
    /// Se llama cuando el UICombat nos indica que le dieron click al boton de regresar estado de acción.
    /// </summary>
    private void OnReturnActionState()
    {
        switch (goUnitActionState)
        {
            case UnitActionState.SelectingCells:
                goUnitActionState = UnitActionState.SelectingUnit;
                DeactivateBoardCellsSelections();

                //<< Le indicamos que botones debe habilitar o no al UI de combate.
                Send(GameCommand.UICombat_DisplayActions, true);
                Send(GameCommand.UICombat_EnableActionAttack, goSelectedBoardUnit.Attacks > 0);
                Send(GameCommand.UICombat_EnableActionMove, goSelectedBoardUnit.Moves > 0);

                //<< Le avisamos al UI de combate que oculte el botón de regresar acción.
                Send(GameCommand.UICombat_DisplayReturnState, false);
                break;
            case UnitActionState.ConfirmingAction:
                goUnitActionState = UnitActionState.SelectingCells;
                DeactivateCellsSelected();
                GameManager.Send(GameCommand.UICombat_DisplayConfirmation, false, goUnitActionMode);
                break;
            default: break;
        }
    }
}

//Seleccionas unidad - Se muestra el area de atque > Seleccionas celda de unidad enemiga para atacar > Aparece confirmar y cancelar
//Seleccionas unidad - Se muestra el area de atque > ? Como se cancela si te arrepientes aqui y quieres atacar con otra unidad

//Dos curadores

//<< Si en el ataque seleccionas otra unidad, se cambia a la unidad seleccionada. No
//<< En curacion solo puedes seleccionar tus propias unidades para curarlas pero que pasa si quieres cambiar de unidad al seleccionar curar?
//<< Ya le diste curar y te arrepientes y quieres seleccionar una unidad circundante?


//<< Se quita el portraretrato al no seleccionar nada.
//<< El secundario de curador.


//<< Las unidades pueden atacar a las aliadas?

//<< Atacar al vacio se quita el retrato de la segunda unidad? Si
//<< Al seleccionar curar se selecciona la unidad a curar propia? Si

//<< Modo selección en tres pasos.
//<< Celda unica seleccionada al terminar la selección.


//<< Si una unidad que no tiene unidades dentro del rango y no puede atacar espacios vacios deberia aparecer el boton de ataque desactivado?
//<< Al igual que para moverse?

//<< Si aparecen las acciones siempre y cuando haya.
//<< Aparece en gris cuando no hay celda de ataque o de movimiento.
//<< Aparece el area completa y en un color las celdas disponibles para atacar o mover.

//<< Unidades pueden seleccionar la celda de una misma unidad cuando estan en rango de ataque?.

//<< Que sucede cuando el tiempo de animacion se excede? Hard update?  Si hard update con corte.