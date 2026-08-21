using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Tablero del juego que se asigna en SignalR.
    /// </summary>
    private static Board<List<BoardUnit>> goBoard;

    [Header("BOARD")]
    [Tooltip("Elemento que contendrá todos los prefabs.")]
    public Transform boardParent;

    [Tooltip("Elemento que se usa como linea para delimitar el mapa.")]
    public Transform gridLine;

    [Tooltip("Elemento que se usa como checker.")]
    public Transform checker;

    [Tooltip("Es el prefab que se usa para los recuadros del tablero para poder usar el raycast en dichos colliders.")]
    public Transform prefabBoardCellSelector;

    [Tooltip("Prefab de la barra de vida de las unidades. (Temp)")]
    public UnitHealthbar prefabUnitHealthbar;

    [Tooltip("Color de la barra de vida del usuario")]
    public Color HealthbarColor;

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
    /// Se asigna el tablero lo cual nos indica que debemos crear todos los elementos del mismo.
    /// </summary>
    /// <param name="loBoard"></param>
    private void SetBoard(Board<List<BoardUnit>> loBoard)
    {
        goBoard = loBoard;

        //<< Creamos el grid del tablero.
        CreateGridAndChecker(loBoard.Width, loBoard.Lenght);

        //<< Creamos las celdas de selección.
        CreateSelectionCells(goBoardProperties.boardWidht, goBoardProperties.boardLenght);

        //<< Instanciamos y posicionamos las unidades del tablero.
        UnitInstance loUnitInstance;
        BoardCell<List<BoardUnit>>[] loCells = loBoard.GetCells();
        foreach (BoardCell<List<BoardUnit>> loCell in loCells)
        {
            foreach (BoardUnit loBoardUnit in loCell.Data)
            {
                //<< Creamos la unidad con base en sus elementos visuales.
                loUnitInstance = CreateUnitInstance(loBoardUnit.UnitProperties);

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
                loUnitHealthbar.barFill.color = HealthbarColor;
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
}