using System;
using System.Collections.Generic;

/// <summary>
/// Se usa para los datos de la celda.
/// </summary>
[Serializable]
public class BoardCell<T> where T : class
{
    [NonSerialized]
    public static readonly BoardCell<T> Blocked = new BoardCell<T>(-1, 0, 0);

    private class IndexDistance
    {
        public int Index;
        public float Distance;
    }

    /// <summary>
    /// Se usa para determinar las distancias entre celdas. Nota: Se usa estatico para evitar tanto GarbageCollector.
    /// </summary>
    [NonSerialized]
    private static IndexDistance[] IndexDistances = new IndexDistance[5] { new IndexDistance(), new IndexDistance(), new IndexDistance(), new IndexDistance(), new IndexDistance() { Distance = float.MaxValue } };

    /// <summary>
    /// Indice de nuestra celda.
    /// </summary>
    [NonSerialized]
    public int Index = 0;

    /// <summary>
    /// Coordenada en X de nuestra celda.
    /// </summary>
    [NonSerialized]
    public int X = 0;

    /// <summary>
    /// Coordenada en Z de nuestra celda.
    /// </summary>
    [NonSerialized]
    public int Z = 0;

    /// <summary>
    /// Cuando se obtiene un area de celdas este valor contiene la distancia apartir del centro del area requerida.
    /// </summary>
    [NonSerialized]
    public int Distance = 0;

    /// <summary>
    /// Datos que contiene la celda.
    /// </summary>
    public List<T> Data = new List<T>();

    /// <summary>
    /// Frente
    /// </summary>
    [NonSerialized]
    public BoardCell<T> ZPos = Blocked;
    /// <summary>
    /// Atrás
    /// </summary>
    [NonSerialized]
    public BoardCell<T> ZNeg = Blocked;
    /// <summary>
    /// Izquierda
    /// </summary>
    [NonSerialized]
    public BoardCell<T> XNeg = Blocked;
    /// <summary>
    /// Derecha
    /// </summary>
    [NonSerialized]
    public BoardCell<T> XPos = Blocked;

    /// <summary>
    /// Diagonal frente izquierda
    /// </summary>
    [NonSerialized]
    public BoardCell<T> ZPosXNeg = Blocked;
    /// <summary>
    /// Diagonal frente derecha
    /// </summary>
    [NonSerialized]
    public BoardCell<T> ZPosXPos = Blocked;

    /// <summary>
    /// Diagonal atrás izquierda.
    /// </summary>
    [NonSerialized]
    public BoardCell<T> ZNegXNeg = Blocked;

    /// <summary>
    /// Diagonal atrás derecha.
    /// </summary>
    [NonSerialized]
    public BoardCell<T> ZNegXPos = Blocked;

    /// <summary>
    /// Son las los alrededores de nuestra celda en cruz.
    /// </summary>
    [NonSerialized]
    public BoardCell<T>[] Cross = new BoardCell<T>[4];

    public BoardCell() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="lnIndex"></param>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    public BoardCell(int lnIndex, int lnX, int lnZ)
    {
        Index = lnIndex;
        X = lnX;
        Z = lnZ;
    }

    /// <summary>
    /// Nos indica si el elemento se encuentra dentro del arreglo.
    /// </summary>
    /// <param name="loElement"></param>
    /// <param name="loList"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    private static bool Contains(BoardCell<T> loElement, ref BoardCell<T>[] loList, int lnCells)
    {
        for (int i = 0; i < lnCells; i++) if (loList[i] == loElement) return true;
        return false;
    }

    #region PATH
    /// <summary>
    /// Nos da posiciones viables con base en el rango/distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="loCaller"></param>
    public static void GetPath(BoardCell<T> loCurrent, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, ref BoardCell<T>[] loArray, ref int lnCells, BoardCell<T> loCaller = null)
    {
        //<< Si ya existe indica que estamos regresando por ello regresamos.
        if (!Contains(loCurrent, ref loArray, lnCells))
        {
            //<< Si no se ha agregado se agrega.
            loCurrent.Distance = lnDistance;
            loArray[lnCells] = loCurrent;
            lnCells++;
        }

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas aledañas en cruz.
        if (loCaller != loCurrent.ZPos && loCurrent.ZPos != null && loPassThrough(loCurrent.ZPos)) GetPath(loCurrent.ZPos, lnDistance, loPassThrough, ref loArray, ref lnCells, loCurrent);
        if (loCaller != loCurrent.ZNeg && loCurrent.ZNeg != null && loPassThrough(loCurrent.ZNeg)) GetPath(loCurrent.ZNeg, lnDistance, loPassThrough, ref loArray, ref lnCells, loCurrent);
        if (loCaller != loCurrent.XPos && loCurrent.XPos != null && loPassThrough(loCurrent.XPos)) GetPath(loCurrent.XPos, lnDistance, loPassThrough, ref loArray, ref lnCells, loCurrent);
        if (loCaller != loCurrent.XNeg && loCurrent.XNeg != null && loPassThrough(loCurrent.XNeg)) GetPath(loCurrent.XNeg, lnDistance, loPassThrough, ref loArray, ref lnCells, loCurrent);
    }

    /// <summary>
    /// Nos da el camino mas acercado con base en la posicion actual y el destino.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="loDestination"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="loCaller"></param>
    /// <returns></returns>
    public static bool GetClosestPath(BoardCell<T> loCurrent, BoardCell<T> loDestination, int lnDistance, Func<BoardCell<T>, bool> loPassThrough, ref BoardCell<T>[] loArray, ref int lnCells, BoardCell<T> loCaller = null)
    {
        //<< Revisamos si hemos encontrado el destino, de ser así avisamos.
        if (loCurrent == loDestination)
        {
            loArray[lnCells] = loCurrent;
            lnCells++;
            return true;
        }

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return false;

        //<< Reinciamos las distancias.
        for (int i = 0; i < 4; i++) IndexDistances[i].Distance = float.MinValue;

        //<< Primero revisamos cual eje es el que está mas acercado para darle prioridad.
        int lnIndexIn;
        float lnDistanceIn;
        BoardCell<T> loCross;
        IndexDistance loIndexDistance;
        int lnIndexTemp;
        float lnDistanceTemp;
        for (int i = 0; i < 4; i++)
        {
            loCross = loCurrent.Cross[i];
            lnDistanceIn = loCross == null ? float.MaxValue : Math.Abs(loCross.X - loDestination.X) + Math.Abs(loCross.Z - loDestination.Z);
            lnIndexIn = i;

            //<< Asignamos su indice.
            for (int j = 0; j < 4; j++)
            {
                //<< Si somos mayor que el siguiente elemento continuamos.
                if (lnDistanceIn > IndexDistances[j + 1].Distance) continue;

                //<< Ya que no somos mayores al siguiente elemento debemos asignarnos a este lugar y empujar al resto a direccion de indice cero ya que el orden es de menor a mayor.
                for (int k = j; k >= 0; k--)
                {
                    loIndexDistance = IndexDistances[k];
                    lnIndexTemp = loIndexDistance.Index;
                    lnDistanceTemp = loIndexDistance.Distance;
                    loIndexDistance.Index = lnIndexIn;
                    loIndexDistance.Distance = lnDistanceIn;
                    lnDistanceIn = lnDistanceTemp;
                    lnIndexIn = lnIndexTemp;
                }
                break;
            }
        }

        //<< Corremos por distancia.
        foreach (IndexDistance loItem in IndexDistances)
        {
            loCross = loCurrent.Cross[loItem.Index];

            //<< Si va a vacio continuamos.
            if (loCross == null) continue;

            //<< No revisamos a quien nos llamó.
            if (loCaller == loCross) continue;

            //<< Si hay unidad aqui continuamos a la que sigue.
            if (!loPassThrough(loCross)) continue;

            //<< Revisamos si desde aqui obtenemos el destino.
            if (!GetClosestPath(loCross, loDestination, lnDistance, loPassThrough, ref loArray, ref lnCells, loCurrent)) continue;

            //<< Ya que si se llegó al objetivo nos agregamos a la lista.
            loArray[lnCells] = loCurrent;
            lnCells++;
            return true;
        }

        return false;
    }
    #endregion

    #region AREA
    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    public static void GetAreaRadial(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells)
    {
        //<< Si ya existe indica que estamos regresando por ello regresamos.
        if (Contains(loCurrent, ref loArray, lnCells)) return;

        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas aledañas en cruz.
        if (loCurrent.ZPos != null) GetAreaRadial(loCurrent.ZPos, lnDistance, ref loArray, ref lnCells, Facing.ZNeg);
        if (loCurrent.ZNeg != null) GetAreaRadial(loCurrent.ZNeg, lnDistance, ref loArray, ref lnCells, Facing.ZPos);
        if (loCurrent.XPos != null) GetAreaRadial(loCurrent.XPos, lnDistance, ref loArray, ref lnCells, Facing.XNeg);
        if (loCurrent.XNeg != null) GetAreaRadial(loCurrent.XNeg, lnDistance, ref loArray, ref lnCells, Facing.XPos);
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="loNegate"></param>
    public static void GetAreaRadial(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loNegate)
    {
        //<< Si ya existe indica que estamos regresando por ello regresamos.
        if (Contains(loCurrent, ref loArray, lnCells)) return;

        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas aledañas en cruz.
        if (loNegate != Facing.ZPos && loCurrent.ZPos != null) GetAreaRadial(loCurrent.ZPos, lnDistance, ref loArray, ref lnCells, loNegate);
        if (loNegate != Facing.ZNeg && loCurrent.ZNeg != null) GetAreaRadial(loCurrent.ZNeg, lnDistance, ref loArray, ref lnCells, loNegate);
        if (loNegate != Facing.XPos && loCurrent.XPos != null) GetAreaRadial(loCurrent.XPos, lnDistance, ref loArray, ref lnCells, loNegate);
        if (loNegate != Facing.XNeg && loCurrent.XNeg != null) GetAreaRadial(loCurrent.XNeg, lnDistance, ref loArray, ref lnCells, loNegate);
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaLineal(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        switch (loFacing)
        {
            case Facing.ZPos: if (loCurrent.ZPos != null) GetAreaLineal(loCurrent.ZPos, lnDistance, ref loArray, ref lnCells, loFacing); break;
            case Facing.ZNeg: if (loCurrent.ZNeg != null) GetAreaLineal(loCurrent.ZNeg, lnDistance, ref loArray, ref lnCells, loFacing); break;
            case Facing.XPos: if (loCurrent.XPos != null) GetAreaLineal(loCurrent.XPos, lnDistance, ref loArray, ref lnCells, loFacing); break;
            case Facing.XNeg: if (loCurrent.XNeg != null) GetAreaLineal(loCurrent.XNeg, lnDistance, ref loArray, ref lnCells, loFacing); break;
            default: break;
        }
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaPerpendicularCentered(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        int lnDistanceHalf = (int)Math.Floor(lnDistance * 0.5f);

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistanceHalf <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        switch (loFacing)
        {
            case Facing.ZPos:
                if (loCurrent.XPos != null) GetAreaLineal(loCurrent.XPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.XPos);
                if (loCurrent.XNeg != null) GetAreaLineal(loCurrent.XNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.XNeg);
                break;
            case Facing.ZNeg:
                if (loCurrent.XNeg != null) GetAreaLineal(loCurrent.XNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.XNeg);
                if (loCurrent.XPos != null) GetAreaLineal(loCurrent.XPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.XPos);
                break;
            case Facing.XPos:
                if (loCurrent.ZPos != null) GetAreaLineal(loCurrent.ZPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZPos);
                if (loCurrent.ZNeg != null) GetAreaLineal(loCurrent.ZNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZNeg);
                break;
            case Facing.XNeg:
                if (loCurrent.ZNeg != null) GetAreaLineal(loCurrent.ZNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZNeg);
                if (loCurrent.ZPos != null) GetAreaLineal(loCurrent.ZPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZPos);
                break;
            default: break;
        }
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaPerpendicularCenteredHollow(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        int lnDistanceHalf = (int)Math.Floor(lnDistance * 0.5f);

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistanceHalf <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        switch (loFacing)
        {
            case Facing.ZPos:
                if (loCurrent.XPos != null) GetAreaLineal(loCurrent.XPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.XPos);
                if (loCurrent.XNeg != null) GetAreaLineal(loCurrent.XNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.XNeg);
                break;
            case Facing.ZNeg:
                if (loCurrent.XNeg != null) GetAreaLineal(loCurrent.XNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.XNeg);
                if (loCurrent.XPos != null) GetAreaLineal(loCurrent.XPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.XPos);
                break;
            case Facing.XPos:
                if (loCurrent.ZPos != null) GetAreaLineal(loCurrent.ZPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZPos);
                if (loCurrent.ZNeg != null) GetAreaLineal(loCurrent.ZNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZNeg);
                break;
            case Facing.XNeg:
                if (loCurrent.ZNeg != null) GetAreaLineal(loCurrent.ZNeg, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZNeg);
                if (loCurrent.ZPos != null) GetAreaLineal(loCurrent.ZPos, lnDistanceHalf, ref loArray, ref lnCells, Facing.ZPos);
                break;
            default: break;
        }
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="loFacing"></param>
    public static BoardCell<T> GetLastLinealCell(BoardCell<T> loCurrent, int lnDistance, Facing loFacing)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return loCurrent;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        switch (loFacing)
        {
            case Facing.ZPos: if (loCurrent.ZPos != null) return GetLastLinealCell(loCurrent.ZPos, lnDistance, loFacing); break;
            case Facing.ZNeg: if (loCurrent.ZNeg != null) return GetLastLinealCell(loCurrent.ZNeg, lnDistance, loFacing); break;
            case Facing.XPos: if (loCurrent.XPos != null) return GetLastLinealCell(loCurrent.XPos, lnDistance, loFacing); break;
            case Facing.XNeg: if (loCurrent.XNeg != null) return GetLastLinealCell(loCurrent.XNeg, lnDistance, loFacing); break;
            default: break;
        }

        return loCurrent;
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaTNormal(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        int lnDistanceHalf = (int)Math.Floor(lnDistance * 0.5f);
        GetAreaLineal(loCurrent, lnDistanceHalf, ref loArray, ref lnCells, loFacing);

        //<< Removemos la ultima celda ya que se agregará nuevamente al obtener el area perpendicular.
        if (lnCells > 0) lnCells--;
        BoardCell<T> loTCurrent = lnCells > 0 ? loArray[lnCells - 1] : loCurrent;
        GetAreaPerpendicularCentered(loTCurrent, lnDistance, ref loArray, ref lnCells, loFacing);
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaTInverted(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        GetAreaPerpendicularCentered(loCurrent, lnDistance, ref loArray, ref lnCells, loFacing);

        int lnDistanceHalf = (int)Math.Floor(lnDistance * 0.5f);

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistanceHalf <= 0) return;

        //<< Ya que tenemos la sección perpendicular tenemos que avanzar en 1 para evitar duplicados.
        switch (loFacing)
        {
            case Facing.ZPos: loCurrent = loCurrent.ZPos; break;
            case Facing.ZNeg: loCurrent = loCurrent.ZNeg; break;
            case Facing.XPos: loCurrent = loCurrent.XPos; break;
            case Facing.XNeg: loCurrent = loCurrent.XNeg; break;
            default: break;
        }

        if (loCurrent != null) GetAreaLineal(loCurrent, lnDistanceHalf, ref loArray, ref lnCells, loFacing);
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaTNormalHollow(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        int lnDistanceHalf = (int)Math.Floor(lnDistance * 0.5f);
        GetAreaLineal(loCurrent, lnDistanceHalf, ref loArray, ref lnCells, loFacing);

        //<< Removemos la ultima celda ya que se agregará nuevamente al obtener el area perpendicular.
        if (lnCells > 0) lnCells--;
        BoardCell<T> loTCurrent = lnCells > 0 ? loArray[lnCells - 1] : loCurrent;
        GetAreaPerpendicularCenteredHollow(loTCurrent, lnDistance, ref loArray, ref lnCells, loFacing);
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaTInvertedHollow(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        GetAreaPerpendicularCenteredHollow(loCurrent, lnDistance, ref loArray, ref lnCells, loFacing);

        int lnDistanceHalf = (int)Math.Floor(lnDistance * 0.5f);

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistanceHalf <= 0) return;

        //<< Ya que tenemos la sección perpendicular tenemos que avanzar en 1 para evitar duplicados.
        switch (loFacing)
        {
            case Facing.ZPos: loCurrent = loCurrent.ZPos; break;
            case Facing.ZNeg: loCurrent = loCurrent.ZNeg; break;
            case Facing.XPos: loCurrent = loCurrent.XPos; break;
            case Facing.XNeg: loCurrent = loCurrent.XNeg; break;
            default: break;
        }

        if (loCurrent != null) GetAreaLineal(loCurrent, lnDistanceHalf, ref loArray, ref lnCells, loFacing);
    }

    /// <summary>
    /// Nos da el area con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    /// <param name="loFacing"></param>
    public static void GetAreaDiagonal(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells, Facing loFacing)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        switch (loFacing)
        {
            case Facing.ZPosXPos: if (loCurrent.ZPosXPos != null) GetAreaDiagonal(loCurrent.ZPosXPos, lnDistance, ref loArray, ref lnCells, loFacing); break;
            case Facing.ZPosXNeg: if (loCurrent.ZPosXNeg != null) GetAreaDiagonal(loCurrent.ZPosXNeg, lnDistance, ref loArray, ref lnCells, loFacing); break;
            case Facing.ZNegXPos: if (loCurrent.ZNegXPos != null) GetAreaDiagonal(loCurrent.ZNegXPos, lnDistance, ref loArray, ref lnCells, loFacing); break;
            case Facing.ZNegXNeg: if (loCurrent.ZNegXNeg != null) GetAreaDiagonal(loCurrent.ZNegXNeg, lnDistance, ref loArray, ref lnCells, loFacing); break;
            default: break;
        }
    }

    /// <summary>
    /// Nos da el area en cruz con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    public static void GetAreaCross(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        GetAreaCrossHollow(loCurrent, lnDistance, ref loArray, ref lnCells);
    }

    /// <summary>
    /// Nos da el area en cruz con base en la distancia y sin la celda central.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    public static void GetAreaCrossHollow(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells)
    {
        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        if (loCurrent.ZPos != null) GetAreaLineal(loCurrent.ZPos, lnDistance, ref loArray, ref lnCells, Facing.ZPos);
        if (loCurrent.ZNeg != null) GetAreaLineal(loCurrent.ZNeg, lnDistance, ref loArray, ref lnCells, Facing.ZNeg);
        if (loCurrent.XPos != null) GetAreaLineal(loCurrent.XPos, lnDistance, ref loArray, ref lnCells, Facing.XPos);
        if (loCurrent.XNeg != null) GetAreaLineal(loCurrent.XNeg, lnDistance, ref loArray, ref lnCells, Facing.XNeg);
    }

    /// <summary>
    /// Nos da el area en X con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    public static void GetAreaX(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        GetAreaXHollow(loCurrent, lnDistance, ref loArray, ref lnCells);
    }

    /// <summary>
    /// Nos da el area en X con base en la distancia.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    public static void GetAreaXHollow(BoardCell<T> loCurrent, int lnDistance, ref BoardCell<T>[] loArray, ref int lnCells)
    {
        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        if (loCurrent.ZPosXPos != null) GetAreaDiagonal(loCurrent.ZPosXPos, lnDistance, ref loArray, ref lnCells, Facing.ZPosXPos);
        if (loCurrent.ZPosXNeg != null) GetAreaDiagonal(loCurrent.ZPosXNeg, lnDistance, ref loArray, ref lnCells, Facing.ZPosXNeg);
        if (loCurrent.ZNegXPos != null) GetAreaDiagonal(loCurrent.ZNegXPos, lnDistance, ref loArray, ref lnCells, Facing.ZNegXPos);
        if (loCurrent.ZNegXNeg != null) GetAreaDiagonal(loCurrent.ZNegXNeg, lnDistance, ref loArray, ref lnCells, Facing.ZNegXNeg);
    }

    /// <summary>
    /// Nos da la lista de celdas enlazadas por otras celdas que contengan algo.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    public static void GetAreaChain(BoardCell<T> loCurrent, int lnDistance, Func<BoardCell<T>, bool> loChainedCell, ref BoardCell<T>[] loArray, ref int lnCells)
    {
        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        if (loCurrent.ZPos != null && loChainedCell(loCurrent.ZPos)) GetAreaChain(loCurrent.ZPos, lnDistance, loChainedCell, ref loArray, ref lnCells, Facing.ZNeg);
        if (loCurrent.ZNeg != null && loChainedCell(loCurrent.ZNeg)) GetAreaChain(loCurrent.ZNeg, lnDistance, loChainedCell, ref loArray, ref lnCells, Facing.ZPos);
        if (loCurrent.XPos != null && loChainedCell(loCurrent.XPos)) GetAreaChain(loCurrent.XPos, lnDistance, loChainedCell, ref loArray, ref lnCells, Facing.XNeg);
        if (loCurrent.XNeg != null && loChainedCell(loCurrent.XNeg)) GetAreaChain(loCurrent.XNeg, lnDistance, loChainedCell, ref loArray, ref lnCells, Facing.XPos);
    }

    /// <summary>
    /// Nos da la lista de celdas enlazadas por otras celdas que contengan algo.
    /// </summary>
    /// <param name="loCurrent"></param>
    /// <param name="lnDistance"></param>
    /// <param name="loArray"></param>
    /// <param name="lnCells"></param>
    private static void GetAreaChain(BoardCell<T> loCurrent, int lnDistance, Func<BoardCell<T>, bool> loChainedCell, ref BoardCell<T>[] loArray, ref int lnCells, Facing loNegate)
    {
        //<< Si ya existe indica que estamos regresando por ello regresamos.
        if (Contains(loCurrent, ref loArray, lnCells)) return;

        //<< Si no se ha agregado se agrega.
        loCurrent.Distance = lnDistance;
        loArray[lnCells] = loCurrent;
        lnCells++;

        //<< Reducimos la distancia.
        lnDistance--;

        //<< Al reducir el desplazamiento en uno y ser cero o menos sabemos que ya no podemos desplazarnos mas entre las celdas.
        if (lnDistance <= 0) return;

        //<< Como aún podemos desplazarnos entre celdas llamamos las funciones de las celdas en linea recta.
        if (loNegate != Facing.ZPos && loCurrent.ZPos != null && loChainedCell(loCurrent.ZPos)) GetAreaChain(loCurrent.ZPos, lnDistance, loChainedCell, ref loArray, ref lnCells, loNegate);
        if (loNegate != Facing.ZNeg && loCurrent.ZNeg != null && loChainedCell(loCurrent.ZNeg)) GetAreaChain(loCurrent.ZNeg, lnDistance, loChainedCell, ref loArray, ref lnCells, loNegate);
        if (loNegate != Facing.XPos && loCurrent.XPos != null && loChainedCell(loCurrent.XPos)) GetAreaChain(loCurrent.XPos, lnDistance, loChainedCell, ref loArray, ref lnCells, loNegate);
        if (loNegate != Facing.XNeg && loCurrent.XNeg != null && loChainedCell(loCurrent.XNeg)) GetAreaChain(loCurrent.XNeg, lnDistance, loChainedCell, ref loArray, ref lnCells, loNegate);
    }

    #endregion

    /// <summary>
    /// Revisa si la celda dada es vecina de nuestra celda.
    /// </summary>
    /// <param name="loBoardCell"></param>
    /// <returns></returns>
    public bool IsNeighbor(BoardCell<T> loBoardCell)
    {
        if (ZPos == loBoardCell) return true;
        if (ZNeg == loBoardCell) return true;
        if (XPos == loBoardCell) return true;
        if (XNeg == loBoardCell) return true;
        return false;
    }

    /// <summary>
    /// Revisa si la celda dada es vecina de nuestra celda.
    /// </summary>
    /// <param name="loBoardCell"></param>
    /// <returns></returns>
    public bool IsNeighbor(BoardCell<T> loBoardCell, out Facing loFacing)
    {
        if (ZPos == loBoardCell) { loFacing = Facing.ZPos; return true; }
        if (ZNeg == loBoardCell) { loFacing = Facing.ZNeg; return true; }
        if (XPos == loBoardCell) { loFacing = Facing.XPos; return true; }
        if (XNeg == loBoardCell) { loFacing = Facing.XNeg; return true; }
        loFacing = Facing.XPos;
        return false;
    }

    /// <summary>
    /// De un area de celdas nos divide por secciones, es decir si en la sección damos 0 nos dará el punto central, si en cambio damos 1 nos dará
    /// las celdas alrededor del punto central, y en el caso de 2 nos daria las celdas alrededor de las del 1.
    /// </summary>
    /// <param name="lnSection">Es la seccion actual de la cual queremos obtener las celdas.</param>
    /// <param name="lnAreaSections">Es el total de secciones del area.</param>
    /// <param name="loAreaCells">Son todas las celdas del area en cuestion.</param>
    /// <param name="loSection">A esta lista se le pasan todas las celdas de la sección.</param>
    /// <param name="lnSelectionCells">Indica la cantidad de celdas de dicha sección.</param>
    public static void GetSectionCells(int lnSection, int lnAreaCells, BoardCell<T>[] loAreaCells, BoardCell<T>[] loSection, out int lnSelectionCells)
    {
        BoardCell<T> loCell;
        lnSelectionCells = 0;

        //<< Obtenemos la sección que nos piden.
        for (int i = 0; i < lnAreaCells; i++)
        {
            loCell = loAreaCells[i];
            if (loCell == null) continue;
            if (loCell.Distance != lnSection) continue;
            loSection[lnSelectionCells] = loCell;
            lnSelectionCells++;
        }
    }

    /// <summary>
    /// Nos da la distancia maxima de las celdas.
    /// </summary>
    /// <param name="lnCells"></param>
    /// <param name="loCells"></param>
    /// <returns></returns>
    public static int GetMaxDistance(int lnCells, BoardCell<T>[] loCells)
    {
        int lnMaxDistance = 0;
        for (int i = 0; i < lnCells; i++)
        {
            if (loCells[i] == null) break;
            if (loCells[i].Distance > lnMaxDistance) lnMaxDistance = loCells[i].Distance;
        }
        return lnMaxDistance;
    }

    /// <summary>
    /// Nos da la celda que se encuentre sobre ZPos mas cercana que cumpla con lo permitido.
    /// </summary>
    /// <param name="lnCellIndex"></param>
    /// <param name="loAllowed"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T> GetLinearCellZPos(Func<BoardCell<T>, bool> loAllowed, ref int lnCells)
    {
        BoardCell<T> loBoardCell = this.ZPos;
        for (int i = 0; i < lnCells; i++)
        {
            //<< Revisamos que no estemos al borde o que no sea permitida la celda.
            if (!loAllowed(loBoardCell))
            {
                lnCells = i;
                return loBoardCell;
            }

            //<< Pasamos a la siguiente celda.
            loBoardCell = loBoardCell.ZPos;
        }

        //<< Regresamos null ya que no hubo celda que detivuera la continuidad.
        return null;
    }

    /// <summary>
    /// Nos da la celda que se encuentre sobre ZNeg mas cercana que cumpla con lo permitido.
    /// </summary>
    /// <param name="lnCellIndex"></param>
    /// <param name="loAllowed"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T> GetLinearCellZNeg(Func<BoardCell<T>, bool> loAllowed, ref int lnCells)
    {
        BoardCell<T> loBoardCell = this.ZNeg;
        for (int i = 0; i < lnCells; i++)
        {
            //<< Revisamos que no estemos al borde o que no sea permitida la celda.
            if (!loAllowed(loBoardCell))
            {
                lnCells = i;
                return loBoardCell;
            }

            //<< Pasamos a la siguiente celda.
            loBoardCell = loBoardCell.ZNeg;
        }

        //<< Regresamos null ya que no hubo celda que detivuera la continuidad.
        return null;
    }

    /// <summary>
    /// Nos da la celda que se encuentre sobre XPos mas cercana que cumpla con lo permitido.
    /// </summary>
    /// <param name="lnCellIndex"></param>
    /// <param name="loAllowed"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T> GetLinearCellXPos(Func<BoardCell<T>, bool> loAllowed, ref int lnCells)
    {
        BoardCell<T> loBoardCell = this.XPos;
        for (int i = 0; i < lnCells; i++)
        {
            //<< Revisamos que no estemos al borde o que no sea permitida la celda.
            if (!loAllowed(loBoardCell))
            {
                lnCells = i;
                return loBoardCell;
            }

            //<< Pasamos a la siguiente celda.
            loBoardCell = loBoardCell.XPos;
        }

        //<< Regresamos null ya que no hubo celda que detivuera la continuidad.
        return null;
    }

    /// <summary>
    /// Nos da la celda que se encuentre sobre XNeg mas cercana que cumpla con lo permitido.
    /// </summary>
    /// <param name="lnCellIndex"></param>
    /// <param name="loAllowed"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public BoardCell<T> GetLinearCellXNeg(Func<BoardCell<T>, bool> loAllowed, ref int lnCells)
    {
        BoardCell<T> loBoardCell = this.XNeg;
        for (int i = 0; i < lnCells; i++)
        {
            //<< Revisamos que no estemos al borde o que no sea permitida la celda.
            if (!loAllowed(loBoardCell))
            {
                lnCells = i;
                return loBoardCell;
            }

            //<< Pasamos a la siguiente celda.
            loBoardCell = loBoardCell.XNeg;
        }

        //<< Regresamos null ya que no hubo celda que detivuera la continuidad.
        return null;
    }
}