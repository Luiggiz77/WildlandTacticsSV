using System;
using System.Collections.Generic;

[Serializable]
public class Board<T> where T : class
{
    /// <summary>
    /// Indica la cantidad de acciones que tiene cada oponente.
    /// </summary>
    public int ActionsPerTurn;

    /// <summary>
    /// Indica la cantidad de ataques que puede realizar una sola unidad por turno.
    /// </summary>
    public int UnitAttacksPerTurn;

    /// <summary>
    /// Indica la cantidad de movimientos que puede realizar una sola unidad por turno.
    /// </summary>
    public int UnitMovesPerTurn;

    /// <summary>
    /// Id del jugador A
    /// </summary>
    public int UserIdA;

    /// <summary>
    /// Id del jugador B
    /// </summary>
    public int UserIdB;

    /// <summary>
    /// Ancho del mapa (cuadrantes).
    /// </summary>
    public int Width;

    /// <summary>
    /// Largo del mapa (cuadrantes).
    /// </summary>
    public int Lenght;

    /// <summary>
    /// Arreglo de celdas lineal.
    /// </summary>
    public BoardCell<T>[] BoardCells;

    /// <summary>
    /// Arreglo de celdas que se usa para cuando se hacen peticiones de celdas.
    /// </summary>
    private BoardCell<T>[] goSelectedCells;

    public Board() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="lnWidth"></param>
    /// <param name="lnLenght"></param>
    public Board(int lnWidth = 6, int lnLenght = 7, int lnActionsPerTurn = 2, int lnUnitAttacksPerTurn = 1, int lnUnitMovesPerTurn = 1)
    {
        ActionsPerTurn = lnActionsPerTurn;
        UnitAttacksPerTurn = lnUnitAttacksPerTurn;
        UnitMovesPerTurn = lnUnitMovesPerTurn;

        Lenght = lnLenght;
        Width = lnWidth;
        BoardCells = CreateBoard(lnWidth, lnLenght);
        int lnSize = lnWidth * lnLenght;
        goSelectedCells = new BoardCell<T>[lnSize];
    }

    /// <summary>
    /// Nos da el index con base en las coordenadas dadas.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <returns></returns>
    public int CellIndexOf(int lnX, int lnZ)
    {
        return Lenght * lnX + lnZ;
    }

    /// <summary>
    /// Nos da la celda con base en el id.
    /// </summary>
    /// <param name="loCellData"></param>
    public BoardCell<T> GetCell(int lnCellIndex)
    {
        return BoardCells[lnCellIndex];
    }

    /// <summary>
    /// Nos da la celda con base en el id pero en un array.
    /// </summary>
    /// <param name="loCellData"></param>
    public BoardCell<T>[] GetCellAsArray(int lnCellIndex)
    {
        goSelectedCells[0] = BoardCells[lnCellIndex];
        return goSelectedCells;
    }

    /// <summary>
    /// Regresa las celdas del tablero.
    /// </summary>
    /// <returns></returns>
    public BoardCell<T>[] GetCells()
    {
        return BoardCells;
    }

    /// <summary>
    /// Nos da todas las celdas del tablero pero como selección.
    /// </summary>
    /// <returns></returns>
    public BoardCell<T>[] GetAreaBoard(out int lnCells)
    {
        lnCells = BoardCells.Length;
        for (int i = 0; i < lnCells; i++) goSelectedCells[i] = BoardCells[i];
        return goSelectedCells;
    }

    /// <summary>
    /// Regresa las celdas del tablero con base en los Id's dados.
    /// </summary>
    /// <returns></returns>
    public BoardCell<T>[] GetCells(IEnumerable<int> loCellIndexes)
    {
        //<< Indicamos que iniciamos con cero celdas.
        int lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        foreach (int lnCellIndex in loCellIndexes)
        {
            goSelectedCells[lnCells] = BoardCells[lnCellIndex];
            lnCells++;
        }

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Regresa las celdas del tablero.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BoardCell<T>> GetCellsOverX(int lnXStart, int lnXEnd, out int lnCells)
    {
        lnCells = 0;
        return Array.Empty<BoardCell<T>>();
    }

    /// <summary>
    /// Nos da los datos que contiene la celda.
    /// </summary>
    /// <param name="loCellData"></param>
    public T GetCellData(int lnCellIndex)
    {
        return BoardCells[lnCellIndex].Data;
    }

    /// <summary>
    /// Se usa para crear el grid con sus maximos.
    /// El primer array representa el ancho.
    /// El segundo array representa el largo.
    /// </summary>
    /// <returns></returns>
    private static BoardCell<T>[] CreateBoard(int lnBoardWidth, int lnBoardLenght)
    {
        //<< Creamos las instancias.
        BoardCell<T>[,] loMap = new BoardCell<T>[lnBoardWidth, lnBoardLenght];
        List<BoardCell<T>> loBoardCells = new List<BoardCell<T>>();
        BoardCell<T> loBoardCell;
        int lnIndex = 0;
        for (int x = 0; x < lnBoardWidth; x++)
        {
            for (int z = 0; z < lnBoardLenght; z++)
            {
                loBoardCell = new BoardCell<T>(lnIndex, x, z);
                loMap[x, z] = loBoardCell;
                loBoardCells.Add(loBoardCell);
                lnIndex++;
            }
        }

        //<< Conectamos al frente y atrás.
        for (int x = 0; x < lnBoardWidth; x++)
        {
            //<< Conectamos Z+
            for (int z = 0; z < lnBoardLenght - 1; z++) loMap[x, z].ZPos = loMap[x, z + 1];

            //<< Conectamos Z-
            for (int z = lnBoardLenght - 1; z >= 1; z--) loMap[x, z].ZNeg = loMap[x, z - 1];
        }

        //<< Conectamos laterales.
        for (int z = 0; z < lnBoardLenght; z++)
        {
            //<< Conectamos X+
            for (int x = 0; x < lnBoardWidth - 1; x++) loMap[x, z].XPos = loMap[x + 1, z];

            //<< Conectamos X-
            for (int x = lnBoardWidth - 1; x >= 1; x--) loMap[x, z].XNeg = loMap[x - 1, z];
        }

        //<< Conectamos todas las diagnonales.
        for (int x = 0; x < lnBoardWidth; x++)
        {
            for (int z = 0; z < lnBoardLenght; z++)
            {
                loBoardCell = loMap[x, z];
                ConnectCross(loBoardCell);
                ConnectDiagonal(loBoardCell);
            }
        }

        return loBoardCells.ToArray();
    }

    /// <summary>
    /// Agrega al array de las celdas en cruz.
    /// </summary>
    /// <param name="loBoardCell"></param>
    private static void ConnectCross(BoardCell<T> loBoardCell)
    {
        loBoardCell.Cross[0] = loBoardCell.ZPos;
        loBoardCell.Cross[1] = loBoardCell.ZNeg;
        loBoardCell.Cross[2] = loBoardCell.XPos;
        loBoardCell.Cross[3] = loBoardCell.XNeg;
    }

    /// <summary>
    /// Conecta las diagonales del GridCell.
    /// </summary>
    /// <param name="loBoardCell"></param>
    private static void ConnectDiagonal(BoardCell<T> loBoardCell)
    {
        //<< Conectamos el frontal izquierdo.
        if (loBoardCell.ZPos != null && loBoardCell.ZPos.XNeg != null) loBoardCell.ZPosXNeg = loBoardCell.ZPos.XNeg;
        else if (loBoardCell.XNeg != null && loBoardCell.XNeg.ZPos != null) loBoardCell.ZPosXNeg = loBoardCell.XNeg.ZPos;

        //<< Conectamos el frontal derecho.
        if (loBoardCell.ZPos != null && loBoardCell.ZPos.XPos != null) loBoardCell.ZPosXPos = loBoardCell.ZPos.XPos;
        else if (loBoardCell.XPos != null && loBoardCell.XPos.ZPos != null) loBoardCell.ZPosXPos = loBoardCell.XPos.ZPos;

        //<< Conectamos la parte trasera izquierda.
        if (loBoardCell.ZNeg != null && loBoardCell.ZNeg.XNeg != null) loBoardCell.ZNegXNeg = loBoardCell.ZNeg.XNeg;
        else if (loBoardCell.XNeg != null && loBoardCell.XNeg.ZNeg != null) loBoardCell.ZNegXNeg = loBoardCell.XNeg.ZNeg;

        //<< Conectamos la parte trasera derecha.
        if (loBoardCell.ZNeg != null && loBoardCell.ZNeg.XPos != null) loBoardCell.ZNegXPos = loBoardCell.ZNeg.XPos;
        else if (loBoardCell.XPos != null && loBoardCell.XPos.ZNeg != null) loBoardCell.ZNegXPos = loBoardCell.XPos.ZNeg;
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima.
    /// </summary>
    /// <returns></returns>
    public void GetPaths(int lnCellIndex, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, List<BoardCell<T>> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetPaths(lnCellIndex, lnDistance, loPassThrough, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i]);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima.
    /// </summary>
    /// <returns></returns>
    public void GetPaths(int lnCellIndex, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, List<int> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetPaths(lnCellIndex, lnDistance, loPassThrough, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i].Index);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima.
    /// </summary>
    /// <returns></returns>
    public BoardCell<T>[] GetPaths(int lnCellIndex, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en el desplazamiento.
        BoardCell<T>.GetPath(BoardCells[lnCellIndex], lnDistance, loPassThrough, ref goSelectedCells, ref lnCells);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area.
    /// </summary>
    /// <returns></returns>
    public void GetAreaRadial(int lnCellIndex, int lnDistance, List<BoardCell<T>> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetAreaRadial(lnCellIndex, lnDistance, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i]);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area.
    /// </summary>
    /// <returns></returns>
    public void GetAreaRadial(int lnCellIndex, int lnDistance, List<int> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetAreaRadial(lnCellIndex, lnDistance, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i].Index);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area en forma de X.
    /// </summary>
    /// <returns></returns>
    public void GetAreaX(int lnCellIndex, int lnDistance, List<BoardCell<T>> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetAreaX(lnCellIndex, lnDistance, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i]);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area en forma de X.
    /// </summary>
    /// <returns></returns>
    public void GetAreaX(int lnCellIndex, int lnDistance, List<int> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetAreaX(lnCellIndex, lnDistance, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i].Index);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area en forma de cruz.
    /// </summary>
    /// <returns></returns>
    public void GetAreaCross(int lnCellIndex, int lnDistance, List<BoardCell<T>> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetAreaCross(lnCellIndex, lnDistance, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i]);
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area en forma de cruz.
    /// </summary>
    /// <returns></returns>
    public void GetAreaCross(int lnCellIndex, int lnDistance, List<int> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetAreaCross(lnCellIndex, lnDistance, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i].Index);
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaRadial(int lnCellIndex, int lnDistance, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaRadial(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;

        //int lnStartX = lnX - lnDistance;
        //int lnStartZ = lnZ - lnDistance;

        //int lnCountX = lnStartX + lnDistance + lnDistance + 1;
        //int lnCountZ = lnStartZ + lnDistance + lnDistance + 1;

        //if (lnStartX < 0) lnStartX = 0;
        //if (lnStartZ < 0) lnStartZ = 0;

        //if (lnCountX > gnWidth) lnCountX = gnWidth;
        //if (lnCountZ > gnLenght) lnCountZ = gnLenght;

        //BoardCell<T> loBoardCell;
        //for (int x = lnStartX; x < lnCountX; x++)
        //{
        //    for (int z = lnStartZ; z < lnCountZ; z++)
        //    {
        //        //<< Si no estamos sobre algun eje revisamos si es viable.
        //        if (x != lnX && z != lnZ)
        //        {
        //            int lnRestX = Mathf.Abs(lnX - x);
        //            int lnRestZ = Mathf.Abs(lnZ - z);
        //            int lnRest = lnRestX + lnRestZ;

        //            //<< Si sobrepasa la distancia continuamos.
        //            if (lnRest > lnDistance) continue;
        //        }

        //        //<< Obtenemos el boardcell.
        //        loBoardCell = goBoard[x, z];

        //        //<< Si está ocupada la celda regresamos.
        //        if (loBoardCell.Data != null) continue;

        //        //<< Agregamos el vector a los viables.
        //        loBoardCells.Add(loBoardCell);
        //    }
        //}
    }

    /// <summary>
    /// Nos da las celdas en forma de cruz con base en la distancia ignorando si hay unidades.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaCross(int lnCellIndex, int lnDistance, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaCross(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas en forma de cruz con base en la distancia ignorando si hay unidades. Nota: No se incluye la celda central.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaCrossHollow(int lnCellIndex, int lnDistance, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaCrossHollow(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas en forma de X con base en la distancia ignorando si hay unidades.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaX(int lnCellIndex, int lnDistance, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaX(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas en forma de X con base en la distancia ignorando si hay unidades. Nota: No se incluye la celda central.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaXHollow(int lnCellIndex, int lnDistance, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaXHollow(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da la lista de celdas enlazadas por otras celdas que contengan algo.
    /// </summary>
    /// <param name="lnCellIndex"></param>
    /// <param name="lnDistance"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T>[] GetAreaChain(int lnCellIndex, int lnDistance, Func<BoardCell<T>, bool> loChainedCell, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaChain(BoardCells[lnCellIndex], lnDistance, loChainedCell, ref goSelectedCells, ref lnCells);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Convierte la distancia de afuera hacia adentro de adentro hacia afuera.
    /// </summary>
    /// <param name="lnCells"></param>
    private void EvaluateDistance(int lnCells, int lnMaxDistance)
    {
        for (int i = 0; i < lnCells; i++) goSelectedCells[i].Distance = lnMaxDistance - goSelectedCells[i].Distance;
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area.
    /// </summary>
    /// <returns></returns>
    public void GetClosestPath(int lnCellIndexStart, int lnCellIndexEnd, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, List<BoardCell<T>> loCells)
    {
        BoardCell<T>[] loSelectedCells = GetClosestPath(lnCellIndexStart, lnCellIndexEnd, lnDistance, loPassThrough, out int lnCells);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i]);
    }

    /// <summary>
    /// Nos da el camino mas cercano para llegar al objetivo.
    /// </summary>
    /// <param name="lnXStart"></param>
    /// <param name="lnZStart"></param>
    /// <param name="lnXEnd"></param>
    /// <param name="lnZEnd"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetClosestPath(int lnCellIndexStart, int lnCellIndexEnd, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, out int lnCells)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en el desplazamiento.
        BoardCell<T>.GetClosestPath(BoardCells[lnCellIndexStart], BoardCells[lnCellIndexEnd], lnDistance, loPassThrough, ref goSelectedCells, ref lnCells);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas accesibles con base en la distancia maxima del area.
    /// </summary>
    /// <returns></returns>
    public void GetAreaLineal(int lnCellIndex, int lnDistance, List<BoardCell<T>> loCells, Facing loFacing)
    {
        BoardCell<T>[] loSelectedCells = GetAreaLineal(lnCellIndex, lnDistance, out int lnCells, loFacing);
        for (int i = 0; i < lnCells; i++) loCells.Add(loSelectedCells[i]);
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaLineal(int lnCellIndex, int lnDistance, out int lnCells, Facing loFacing)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaLineal(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells, loFacing);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loBoardCells"></param>
    public BoardCell<T>[] GetAreaPerpendicularCentered(int lnCellIndex, int lnDistance, out int lnCells, Facing loFacing)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaPerpendicularCentered(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells, loFacing);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    public BoardCell<T>[] GetAreaTNormal(int lnCellIndex, int lnDistance, out int lnCells, Facing loFacing)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaTNormal(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells, loFacing);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    public BoardCell<T>[] GetAreaTInverted(int lnCellIndex, int lnDistance, out int lnCells, Facing loFacing)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaTInverted(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells, loFacing);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    public BoardCell<T>[] GetAreaTNormalHollow(int lnCellIndex, int lnDistance, out int lnCells, Facing loFacing)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaTNormalHollow(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells, loFacing);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la distancia ignorando si hay unidades.
    /// </summary>
    public BoardCell<T>[] GetAreaTInvertedHollow(int lnCellIndex, int lnDistance, out int lnCells, Facing loFacing)
    {
        //<< Indicamos que iniciamos con cero celdas seleccionadas.
        lnCells = 0;

        //<< Obtenemos la celda de inicio con base en las coordenadas dadas e indicamos a la celda que realize la busqueda del rango con base en la distancia.
        BoardCell<T>.GetAreaTInvertedHollow(BoardCells[lnCellIndex], lnDistance, ref goSelectedCells, ref lnCells, loFacing);

        //<< Establecemos la distancia apartir del centro/inicio.
        EvaluateDistance(lnCells, lnDistance);

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da una unica celda.
    /// </summary>
    /// <param name="lnCellIndex"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T>[] GetAreaSingle(int lnCellIndex, out int lnCells)
    {
        lnCells = 1;
        goSelectedCells[0] = BoardCells[lnCellIndex];
        goSelectedCells[0].Distance = 0;
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la definición de la "mascara" o funcion ingresada.
    /// </summary>
    /// <param name="loAllowed">True nos indica que si se mantiene en la lista y false indica que se remueve de la lista.</param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T>[] GetCells(Func<BoardCell<T>, bool> loAllowed, ref int lnCells)
    {
        //<< Si no hay mascara regresamos las celdas completas.
        if (loAllowed == null) return goSelectedCells;

        //<< Removemos las celdas a las cuales se les aplica mascara.
        int lnRemovedCells = 0;
        for (int i = 0; i < lnCells; i++)
        {
            if (loAllowed(goSelectedCells[i])) continue;
            goSelectedCells[i] = BoardCell<T>.Blocked;
            lnRemovedCells++;
        }

        //<< Si no se removieron celdas solo regresamos todo integro.
        if (lnRemovedCells == 0) return goSelectedCells;

        //<< Removemos los espacios vacios y determinamos la cantidad de celdas restantes.
        for (int i = 0; i < lnCells; i++)
        {
            if (goSelectedCells[i] != null) continue;
            for (int j = i + 1; j < lnCells; j++)
            {
                if (goSelectedCells[j] == null) continue;
                goSelectedCells[i] = goSelectedCells[j];
                goSelectedCells[j] = BoardCell<T>.Blocked;
                break;
            }
        }

        //<< Determinamos la cantidad de celdas.
        lnCells -= lnRemovedCells;

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la definición de la "mascara" o funcion ingresada.
    /// </summary>
    /// <param name="loAllowed">True nos indica que si se mantiene en la lista y false indica que se remueve de la lista.</param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T>[] GetCells<T1>(Func<BoardCell<T>, T1, bool> loAllowed, T1 loT1, ref int lnCells)
    {
        //<< Si no hay mascara regresamos las celdas completas.
        if (loAllowed == null) return goSelectedCells;

        //<< Removemos las celdas a las cuales se les aplica mascara.
        int lnRemovedCells = 0;
        for (int i = 0; i < lnCells; i++)
        {
            if (loAllowed(goSelectedCells[i], loT1)) continue;
            goSelectedCells[i] = BoardCell<T>.Blocked;
            lnRemovedCells++;
        }

        //<< Si no se removieron celdas solo regresamos todo integro.
        if (lnRemovedCells == 0) return goSelectedCells;

        //<< Removemos los espacios vacios y determinamos la cantidad de celdas restantes.
        for (int i = 0; i < lnCells; i++)
        {
            if (goSelectedCells[i] != null) continue;
            for (int j = i + 1; j < lnCells; j++)
            {
                if (goSelectedCells[j] == null) continue;
                goSelectedCells[i] = goSelectedCells[j];
                goSelectedCells[j] = BoardCell<T>.Blocked;
                break;
            }
        }

        //<< Determinamos la cantidad de celdas.
        lnCells -= lnRemovedCells;

        //<< Mandamos las celdas seleccionadas.
        return goSelectedCells;
    }

    /// <summary>
    /// Nos da las celdas con base en la definición de la "mascara" o funcion ingresada.
    /// </summary>
    /// <param name="loAllowed">True nos indica que si se mantiene en la lista y false indica que se remueve de la lista.</param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public static void FilterCells<T1, T2>(BoardCell<T>[] loCells, Func<BoardCell<T>, T1, T2, bool> loAllowed, T1 loT1, T2 loT2, ref int lnCells)
    {
        //<< Si no hay mascara regresamos las celdas completas.
        if (loAllowed == null) return;

        //<< Removemos las celdas a las cuales se les aplica mascara.
        int lnRemovedCells = 0;
        for (int i = 0; i < lnCells; i++)
        {
            if (loAllowed(loCells[i], loT1, loT2)) continue;
            loCells[i] = BoardCell<T>.Blocked;
            lnRemovedCells++;
        }

        //<< Si no se removieron celdas solo regresamos todo integro.
        if (lnRemovedCells == 0) return;

        //<< Removemos los espacios vacios y determinamos la cantidad de celdas restantes.
        for (int i = 0; i < lnCells; i++)
        {
            if (loCells[i] != null) continue;
            for (int j = i + 1; j < lnCells; j++)
            {
                if (loCells[j] == null) continue;
                loCells[i] = loCells[j];
                loCells[j] = BoardCell<T>.Blocked;
                break;
            }
        }

        //<< Determinamos la cantidad de celdas.
        lnCells -= lnRemovedCells;
    }

    /// <summary>
    /// Nos da una sola celda aleatoria de las que se tienen en el arreglo.
    /// </summary>
    /// <param name="loCells"></param>
    /// <param name="lnCells"></param>
    public static void GetRandomCell(BoardCell<T>[] loCells, ref int lnCells)
    {
        if (lnCells == 0) return;
        int lnIndex = UnityEngine.Random.Range(0, lnCells);
        loCells[0] = loCells[lnIndex];
        lnCells = 1;
    }

    /// <summary>
    /// Nos da las celdas con base en la definición de la "mascara" o funcion ingresada.
    /// </summary>
    /// <param name="loAllowed">True nos indica que si se mantiene en la lista y false indica que se remueve de la lista.</param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public static void FilterCells<T1, T2>(IList<BoardCell<T>> loCells, Func<BoardCell<T>, T1, T2, bool> loAllowed, T1 loT1, T2 loT2)
    {
        //<< Si no hay mascara regresamos las celdas completas.
        if (loAllowed == null) return;

        //<< Removemos las celdas a las cuales se les aplica mascara.
        int lnCells = loCells.Count;
        for (int i = lnCells - 1; i >= 0; i--)
        {
            if (loAllowed(loCells[i], loT1, loT2)) continue;
            loCells.RemoveAt(i);
        }
    }

    /// <summary>
    /// Nos da una celda aleatoria.
    /// </summary>
    /// <returns></returns>
    public int GetRandomCellIndex()
    {
        return UnityEngine.Random.Range(0, BoardCells.Length - 1);
    }

    /// <summary>
    /// Nos da la celda mas lejana en la selección.
    /// </summary>
    /// <param name="loAllowed"></param>
    /// <returns></returns>
    public BoardCell<T>[] FurthestCell<T1, T2>(Func<BoardCell<T>, T1, T2, bool> loAllowed, T1 loT1, T2 loT2, int lnMaxDistance, int lnCellIndex, out int lnCells)
    {
        int lnCount = BoardCells.Length - 1;
        int lnDistance;
        lnCells = 0;
        goSelectedCells[0] = BoardCell<T>.Blocked;
        BoardCell<T> loCell = BoardCells[lnCellIndex];
        int lnX = loCell.X;
        int lnZ = loCell.Z;
        for (int i = 0; i < lnCount; i++)
        {
            loCell = BoardCells[i];
            if (!loAllowed(loCell, loT1, loT2)) continue;
            lnDistance = Math.Abs(lnX - loCell.X) + Math.Abs(lnZ - loCell.Z);
            if (lnDistance < lnMaxDistance) continue;
            goSelectedCells[0] = loCell;
            lnMaxDistance = lnDistance;
        }

        //<< Si hay selección avisamos.
        if (goSelectedCells[0] != null)
        {
            goSelectedCells[0].Distance = 0;
            lnCells = 1;
        }

        return goSelectedCells;
    }

    /// <summary>
    /// Nos da la celda mas cercana en la selección.
    /// </summary>
    /// <param name="loAllowed"></param>
    /// <returns></returns>
    public BoardCell<T>[] NearestCell<T1, T2>(Func<BoardCell<T>, T1, T2, bool> loAllowed, T1 loT1, T2 loT2, int lnMinDistance, int lnCellIndex, out int lnCells)
    {
        int lnCount = BoardCells.Length - 1;
        int lnDistance;
        lnCells = 0;
        goSelectedCells[0] = BoardCell<T>.Blocked;
        BoardCell<T> loCell = BoardCells[lnCellIndex];
        int lnX = loCell.X;
        int lnZ = loCell.Z;
        for (int i = 0; i < lnCount; i++)
        {
            loCell = BoardCells[i];
            if (!loAllowed(loCell, loT1, loT2)) continue;
            lnDistance = Math.Abs(lnX - loCell.X) + Math.Abs(lnZ - loCell.Z);
            if (lnDistance > lnMinDistance) continue;
            goSelectedCells[0] = loCell;
            lnMinDistance = lnDistance;
        }

        //<< Si hay selección avisamos.
        if (goSelectedCells[0] != null)
        {
            goSelectedCells[0].Distance = 0;
            lnCells = 1;
        }

        return goSelectedCells;
    }
}