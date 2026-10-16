using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [Tooltip("Color de la barra de vida del usuario")]
    public Color HealthbarColor;

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
    /// Bandera que nos indica que el tablero está activo para input.
    /// </summary>
    private bool gbBoardInteraction = false;

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
    private int gnSelectedUnitPropertiesId = 0;

    /// <summary>
    /// Es la ultima unidad del tablero seleccionada del oponente.
    /// </summary>
    private int gnSelectedUnitPropertiesIdOpponent = 0;

    /// <summary>
    /// Se asigna el tablero lo cual nos indica que debemos crear todos los elementos del mismo.
    /// </summary>
    /// <param name="loBoard"></param>
    private void SetBoard(Board<BoardUnit> loBoard)
    {
        goBoard = loBoard;
        goBoard.ConnectBoard();

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
        if (!gbBoardInteraction) return;

        //<< Si nos indican que debemos deplazar la unidad seleccionada solo regresamos durante este proceso.
        if (gnDeltaCellPath > -1) return;

        //<< Recibimos el evento de ondown.
        if (!Input.GetMouseButtonDown(0)) return;

        gnMousePositionMagnitude = Input.mousePosition.magnitude;

        //<< Desactivamos la celda vacia.
        boardCellSelectedEmpty.gameObject.SetActive(false);

        //<< Determinamos cual fue la celda seleccionada.
        goRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(goRay, out goRaycastHit, Mathf.Infinity, Layer.BoardCellMask)) return;

        //<< Seleccionamos la celda. Nota: Sabemos que el nombre del GameObject es el index de la celda.
        int lnCellIndex = int.Parse(goRaycastHit.collider.gameObject.name);
        SelectCell(lnCellIndex);
    }

    /// <summary>
    /// Para cuando deseamos seleccionar una celda con base en su cellindex. (Se usa para la IA también)
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
        if (loSelectedBoardUnit != null && (loSelectedBoardUnit.UnitProperties.Id == gnSelectedUnitPropertiesId || loSelectedBoardUnit.UnitProperties.Id == gnSelectedUnitPropertiesIdOpponent)) return;
        
        if (loSelectedBoardUnit.UserId == gnUserId) gnSelectedUnitPropertiesId = loSelectedBoardUnit.UnitProperties.Id;
        else gnSelectedUnitPropertiesIdOpponent = loSelectedBoardUnit.UnitProperties.Id;

        //<< Avisamos de nueva selección.
        Send(GameCommand.SetSelectedUnit, gnUserId, loSelectedBoardUnit);
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
}