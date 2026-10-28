using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class GameManager : MonoBehaviour
{
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
    public Color unitOutlineColorOpponent = Color.orangeRed;

    [Header("CAMERA")]
    [Tooltip("Es la camara que muestra el loop de ataque.")]
    public GameObject cameraUnitAttackLoop;

    [Tooltip("Es la camara que muestra el portraretrato de la unidad del usuario.")]
    public GameObject cameraUnitPortraitUser;

    [Tooltip("Es la camara que muestra el portraretrato de la unidad del oponente.")]
    public GameObject cameraUnitPortraitOpponent;

    /// <summary>
    /// Nuestra celda de unidad seleccionada.
    /// </summary>
    private int gnSelectedCellIndex = -1;

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
    private BoardUnit goSelectedBoardUnitOpponent = null;

    /// <summary>
    /// Es el clon de la unidad usada para el portaretrato.
    /// </summary>
    private UnitInstance goUnitInstancePortraitUser = null;

    /// <summary>
    /// Es el clon de la unidad usada para el portaretrato.
    /// </summary>
    private UnitInstance goUnitInstancePortraitOpponent = null;

    private static MaterialPropertyBlock goMPB;

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
            case GameCommand.OnBoardMouseDown: OnBoardMouseDown(); break;
            case GameCommand.OnBoardMouseDrag: OnBoardMouseDrag(); break;
            case GameCommand.OnBoardMouseUp: OnBoardMouseUp(); break;
            case GameCommand.SetUnitActionMode: OnSetActionMode((UnitActionMode)loParams[0]); break;
            case GameCommand.CancelUnitAction: DeactivateCellsSelected(); break;
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
    /// Evento de OnClick en sobre el tablero para seleccionar u otras cosas.
    /// Nota: Esta funcion se llama en los eventos del panel de UICombat atraves de llamadas del GameCommand.
    /// </summary>
    private void OnBoardMouseDown()
    {
        //<< Si nos indican que debemos deplazar la unidad seleccionada solo regresamos durante este proceso.
        if (gnDeltaCellPath > -1) return;

        //<< Recibimos el evento de ondown.
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 loMousePosition = Mouse.current.position.ReadValue();
        gnMousePositionMagnitude = loMousePosition.magnitude;

        //<< Desactivamos la celda vacia.
        boardCellSelectedEmpty.gameObject.SetActive(false);

        //<< Determinamos cual fue la celda seleccionada.
        goRay = Camera.main.ScreenPointToRay(loMousePosition);
        if (!Physics.Raycast(goRay, out goRaycastHit, Mathf.Infinity, Layer.BoardCellMask)) return;

        //<< Seleccionamos la celda. Nota: Sabemos que el nombre del GameObject es el index de la celda.
        int lnCellIndex = int.Parse(goRaycastHit.collider.gameObject.name);
        SelectCell(lnCellIndex);
    }

    /// <summary>
    /// Para cuando deseamos seleccionar una celda con base en su cellindex.
    /// </summary>
    private void SelectCell(int lnCellIndex)
    {
        //<< Desactivamos la celda vacia.
        boardCellSelectedEmpty.gameObject.SetActive(false);

        //<< Obtenemos la celda seleccionada.
        BoardCell<BoardUnit> loBoardCell = goBoard.GetCell(lnCellIndex);

        //<< Obtenemos la unidad de la celda seleccionada.
        BoardUnit loSelectedBoardUnit = loBoardCell.Data.FirstOrDefault(x => x.UnitProperties.UnitBattleProperties.UnitType == UnitType.Unit);

        //<< Si en la celda no hay unidad seleccionable posicionamos el elemento de celda vacia y regresamos.
        if (loSelectedBoardUnit == null)
        {
            //<< Mostramos la celda vacia.
            boardCellSelectedEmpty.gameObject.SetActive(true);
            boardCellSelectedEmpty.transform.position = new Vector3(loBoardCell.X, 0.0f, loBoardCell.Z);
            return;
        }

        //<< Si es la celda de nuestra unidad ya seleccionada regresamos.
        if (loSelectedBoardUnit != null && (loSelectedBoardUnit == goSelectedBoardUnit || loSelectedBoardUnit == goSelectedBoardUnitOpponent)) return;

        //<< Avisamos de nueva selección.
        if (loSelectedBoardUnit.UserId == gnUserId)
        {
            gnSelectedCellIndex = lnCellIndex;
            goSelectedBoardUnit = loSelectedBoardUnit;
            Send(GameCommand.SetSelectedUnit, gnUserId, loSelectedBoardUnit, unitOutlineColorUser);

            //<< Creamos la unidad del portaretrato.
            if (goUnitInstancePortraitUser == null || goUnitInstancePortraitUser.UnitProperties.Id != loSelectedBoardUnit.UnitProperties.Id) CreatePortraitUnit(loSelectedBoardUnit.UnitProperties, goBoard.UserIdA == gnUserId ? goBoard.FactionUserA : goBoard.FactionUserB, cameraUnitPortraitUser, ref goUnitInstancePortraitUser, Layer.UnitPortraitUser);

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
            goSelectedBoardUnitOpponent = loSelectedBoardUnit;
            Send(GameCommand.SetSelectedUnit, gnUserId, loSelectedBoardUnit, unitOutlineColorOpponent);

            //<< Creamos la unidad del portaretrato.
            if (goUnitInstancePortraitOpponent == null || goUnitInstancePortraitOpponent.UnitProperties.Id != loSelectedBoardUnit.UnitProperties.Id) CreatePortraitUnit(loSelectedBoardUnit.UnitProperties, goBoard.UserIdA == gnUserId ? goBoard.FactionUserA : goBoard.FactionUserB, cameraUnitPortraitOpponent, ref goUnitInstancePortraitOpponent, Layer.UnitPortraitOpponent);
        }

        //<< Si no hay turnos no aparecen las celdas.
        if (goBoard.GetActionsCount(gnUserId) <= 0) return;

        //<< Creamos las celdas con base en la acción a realizar.
        CreateActionCells(lnCellIndex, loSelectedBoardUnit);
    }

    /// <summary>
    /// Crea las celdas necesarias con base en la accion que deseamos realizar con la unidad.
    /// </summary>
    /// <param name="loUnitInstance"></param>
    private void CreateActionCells(int lnCellIndex, BoardUnit loBoardUnit)
    {
        //<< Desactivamos cualquier celda de selección previa.
        DeactivateBoardCellsSelections();

        //<< Desactivamos cualquier celda de area de efecto previa.
        DeactivateCellsEffectArea();

        switch (goUnitActionMode)
        {
            case UnitActionMode.Move:
                {
                    //<< Revisamos si tiene movimientos disponibles.
                    if (loBoardUnit.Moves == 0) return;

                    //<< Obtenemos las celdas por las que puede moverse la unidad.
                    goBoard.GetPaths(lnCellIndex, loBoardUnit.GetMovement() + 1, BoardCellPassThrough, goUnitSurroundingCells);

                    //<< Creamos las celdas usando el rango de movimiento.
                    CreateSurroundingCells(prefabBoardCellMove, Color.green);
                }
                break;
            case UnitActionMode.Attack:
                {
                    //<< Revisamos si tiene ataques disponibles.
                    if (loBoardUnit.Attacks == 0) return;

                    int lnAttackRange = loBoardUnit.GetAttackRange() + 1;

                    //<< Obtenemos las celdas a las que puede atacar la unidad.
                    switch (loBoardUnit.UnitProperties.UnitBattleProperties.AttackAreaType)
                    {
                        case AttackAreaType.Cross: goBoard.GetAreaCross(lnCellIndex, lnAttackRange, goUnitSurroundingCells); break;
                        case AttackAreaType.X: goBoard.GetAreaX(lnCellIndex, lnAttackRange, goUnitSurroundingCells); break;
                        default: goBoard.GetAreaRadial(lnCellIndex, lnAttackRange, goUnitSurroundingCells); break;
                    }

                    //<< Creamos las celdas usando el rango de ataque.
                    CreateSurroundingCells(prefabBoardCellAttack, Color.red);
                }
                break;
            default: break;
        }
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
                            else
                            {
                                //<< Obtenemos el hashcode de la celda que representa las celdas de desplazamiento.
                                int lnHashCodeIn = prefabBoardCellSelected.GetHashCode();
                                Transform loCellSelectedIn;
                                lnIndexOfBoardCell++;
                                int lnCount = goUnitCellsSelected.Count - lnIndexOfBoardCell;
                                if (lnCount > 0) goUnitCellsSelected.RemoveRange(lnIndexOfBoardCell, lnCount);

                                //<< Removemos las instancias de las celdas del camino seleccionado.
                                DeactivateSpawns(prefabBoardCellSelected.GetHashCode());

                                //<< Creamos nuevas instancias de camino seleccionado.
                                foreach (BoardCell<BoardUnit> loItem in goUnitCellsSelected)
                                {
                                    //<< Instanciamos la celda seleccionada.
                                    loCellSelectedIn = Spawn(lnHashCodeIn, prefabBoardCellSelected);
                                    loCellSelectedIn.position = new Vector3(loItem.X, 0.0f, loItem.Z);
                                }

                                return;
                            }
                        }
                    }

                    //<< Si ya tenemos el maximo de celdas regresamos.
                    if (goSelectedBoardUnit.GetMovement() < goUnitCellsSelected.Count) return;

                    //<< En caso de tener la lista vacia solo aceptamos que la celda seleccionada sea la que contiene la unidad.
                    if (goUnitCellsSelected.Count == 0)
                    {
                        if (goUnitSurroundingCells[0] != loBoardCell) return;
                    }
                    else if (!goUnitCellsSelected[goUnitCellsSelected.Count - 1].IsNeighbor(loBoardCell)) return;//<< Revisamos si es vecino de la ultima celda seleccionada.

                    //<< Agregamos la celda seleccionada ya que cumple con los requisitos.
                    goUnitCellsSelected.Add(loBoardCell);

                    //<< Obtenemos el hashcode de la celda que representa las celdas de desplazamiento.
                    int lnHashCode = prefabBoardCellSelected.GetHashCode();

                    //<< Instanciamos la celda seleccionada.
                    Transform loCellSelected = Spawn(lnHashCode, prefabBoardCellSelected);
                    loCellSelected.position = new Vector3(loBoardCell.X, 0.0f, loBoardCell.Z);
                }
                break;
            case UnitActionMode.Attack:
                loBoardCell = goBoard.GetCell(lnCellIndex);
                OnUnitActionModeAttack(loBoardCell, goSelectedBoardUnit);
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
        //<< Si nos indican que debemos deplazar la unidad seleccionada solo regresamos durante este proceso.
        if (gnDeltaCellPath > -1) return;

        if (Mouse.current.leftButton.wasReleasedThisFrame) OnLeftMouseButtonUp();
    }

    /// <summary>
    /// Se llama cuando levantamos el boton izquierdo del mouse.
    /// </summary>
    private void OnLeftMouseButtonUp()
    {
        //<< Si no corresponde la unidad con el turno regresamos.
        if (goSelectedBoardUnit == null || goSelectedBoardUnit.UserId != gnUserId) return;

        switch (goUnitActionMode)
        {
            case UnitActionMode.Move:
                //<< Si solo es una celda sabemos que es la misma celda en la que está la unidad por ello debemos detener el flujo.
                if (goUnitCellsSelected.Count == 1) DeactivateCellsSelected();

                //<< Avisamos que debemos habilitar o deshabilitar el botón de confirmación.
                Send(GameCommand.UICombat_DisplayConfirmation, goUnitCellsSelected.Count > 1, goUnitActionMode);
                break;
            case UnitActionMode.Attack:
                //<< Si es una unidad con rango cero sabemos que debemos agregar la misma celda en la que se encuentra y seleccionar el area.
                if (goSelectedBoardUnit.GetAttackRange() == 0)
                {
                    OnUnitZeroRange(goSelectedBoardUnit);
                }
                //<< Si solo es una celda y es la misma celda en la que está la unidad debemos detener el flujo.
                else if (goUnitCellsSelected.Count > 1) DeactivateCellsSelected();

                //<< Revisamos si esta unidad puede atacar celdas vacias, de no ser así y que no haya unidades en la celda desactivamos la selección.
                else if (!goSelectedBoardUnit.UnitProperties.UnitBattleProperties.AllowAttackEmptySpace && !goUnitCellsSelected.Any(x => x.Data.Count > 0 && x.Data.Any(x => x.UserId == gnUserId))) DeactivateCellsSelected();

                //<< Avisamos que debemos habilitar o deshabilitar el botón de confirmación.
                Send(GameCommand.UICombat_DisplayConfirmation, goUnitCellsSelected.Count == 1, goUnitActionMode);
                break;
            case UnitActionMode.Heal:
                //<< TODO: Falta meter la logica de curación aqui.
                break;
            default: break;
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
        OnUnitActionModeAttack(loBoardCell, loBoardUnit);
        goUnitSurroundingCells.Remove(loBoardCell);
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
    private static void CreateSurroundingCells<T>(T loPrefabBoardCell, Color loColor) where T : Component
    {
        //<< Si no hay celdas solo regresamos.
        if (goUnitSurroundingCells.Count <= 1)
        {
            goUnitSurroundingCells.Clear();
            return;
        }

        //<< Obtenemos el hashcode del prefab que representa las celdas.
        int lnHashCode = loPrefabBoardCell.GetHashCode();

        //<< Instanciamos el rango de celdas.
        T loCell;
        bool lbNew;
        BoardCell<BoardUnit> loBoardCellPath;
        int lnCount = goUnitSurroundingCells.Count;
        for (int i = 1; i < lnCount; i++)
        {
            loBoardCellPath = goUnitSurroundingCells[i];
            loCell = Spawn(lnHashCode, loPrefabBoardCell, out lbNew);
            loCell.transform.position = new Vector3(loBoardCellPath.X, 0.0f, loBoardCellPath.Z);
            if (!lbNew) continue;
            SetBoardElementColor(loCell, loColor);
        }
    }

    private void OnUnitActionModeAttack(BoardCell<BoardUnit> loBoardCellTarget, BoardUnit loBoardUnit)
    {
        //<< Revisamos si es una de las celdas circundantes, de ser así la seleccionamos, de lo contrario regresamos.
        if (!goUnitSurroundingCells.Contains(loBoardCellTarget)) return;

        //<< Desactivamos las celdas previamente seleccionadas.
        DeactivateCellsSelected();

        //<< Agregamos la celda seleccionada ya que cumple con los requisitos.
        goUnitCellsSelected.Add(loBoardCellTarget);

        //<< Obtenemos el hashcode de la celda que representa las celdas de ataque.
        int lnHashCode = prefabBoardCellSelected.GetHashCode();

        //<< Instanciamos la celda seleccionada.
        Transform loCellSelected = Spawn(lnHashCode, prefabBoardCellSelected);
        loCellSelected.position = new Vector3(loBoardCellTarget.X, 0.0f, loBoardCellTarget.Z);

        //<< Mostramos el area de efecto.
        CreateEffectAreaOf(loBoardUnit, loBoardCellTarget, loBoardUnit.GameplayEffectsAttack);
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
    /// Se llama cuando se establece el modo de acción.
    /// </summary>
    /// <param name="loUnitActionMode"></param>
    private void OnSetActionMode(UnitActionMode loUnitActionMode)
    {
        goUnitActionMode = loUnitActionMode;
        CreateActionCells(gnSelectedCellIndex, goSelectedBoardUnit);
    }
}