using System;
using System.Collections.Generic;

[Serializable]
public class DistributionBoard
{
    /// <summary>
    /// Id de nuestro tablero de distribución.
    /// </summary>
    public int Id = 0;

    /// <summary>
    /// Nombre de nuestro tablero de distribución.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Largo de nuestro tablero.
    /// </summary>
    private int gnLength = 0;

    /// <summary>
    /// Nuestro tablero de distribución de unidades, 0 indica que no hay nada y un numero indica el id de la unidad.
    /// </summary>
    private int[] goBoard = new int[0];

    /// <summary>
    /// Se llama para configurar
    /// </summary>
    /// <param name="lnWidth"></param>
    /// <param name="lnLength"></param>
    public void Setup(int lnWidth, int lnLength)
    {
        gnLength = lnLength;
        goBoard = new int[lnWidth * lnLength];
    }

    /// <summary>
    /// Nos da el id de la unidad en el vector dado.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <returns></returns>
    public int GetIdOf(int lnX, int lnZ)
    {
        int lnIndex = IndexOf(lnX, lnZ);
        return goBoard[lnIndex];
    }

    /// <summary>
    /// Nos da el indice de las coordenadas dentro del array unidimensional.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <returns></returns>
    private int IndexOf(int lnX, int lnZ)
    {
        return (lnX * gnLength) + lnZ;
    }

    /// <summary>
    /// Nos da las cooredenadas del index dado.
    /// </summary>
    /// <param name="lnIndex"></param>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    private void CoordinatesOf(int lnIndex, out int lnX, out int lnZ)
    {
        lnX = UnityEngine.Mathf.FloorToInt((float)lnIndex / gnLength);
        lnZ = lnIndex - (gnLength * lnX);
    }

    /// <summary>
    /// Nos indica si el id se encuentra dentro del array.
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    /// <returns></returns>
    public bool Contains(int lnUnitPropertiesId)
    {
        if (goBoard == null) return false;
        foreach (int lnCellId in goBoard)
        {
            if (lnCellId == lnUnitPropertiesId) return true;
        }
        return false;
    }

    /// <summary>
    /// Se llama para remover una unidad del mapa de distribución con base en el id dado.
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    public void AddUnit(int lnUnitPropertiesId, int lnX, int lnZ)
    {
        goBoard[IndexOf(lnX, lnZ)] = lnUnitPropertiesId;
    }

    /// <summary>
    /// Se llama para remover una unidad del mapa de distribución con base en el id dado.
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    public bool RemoveUnit(int lnUnitPropertiesId)
    {
        int lnCount = goBoard.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (goBoard[i] != lnUnitPropertiesId) continue;
            goBoard[i] = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Se llama para remover una unidad del mapa de distribución con base en el id dado.
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    public bool RemoveUnit(int lnUnitPropertiesId, out int lnX, out int lnZ)
    {
        int lnCount = goBoard.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (goBoard[i] != lnUnitPropertiesId) continue;
            goBoard[i] = 0;
            CoordinatesOf(i, out lnX, out lnZ);
            return true;
        }
        lnX = 0;
        lnZ = 0;
        return false;
    }

    /// <summary>
    /// Nos indica la cantidad de unidades en el tablero.
    /// </summary>
    /// <returns></returns>
    public int GetUnitsCount()
    {
        int lnCountUnits = 0;
        int lnCount = goBoard.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (goBoard[i] > 0) lnCountUnits++;
        }
        return lnCountUnits;
    }

    /// <summary>
    /// Nos da las coordenadas de las unidades asi como su id de cada una.
    /// </summary>
    /// <returns></returns>
    public List<DistributionBoardUnitCoordinates> GetUnitsCoordinates()
    {
        List<DistributionBoardUnitCoordinates> loList = new List<DistributionBoardUnitCoordinates>();

        int lnUnitPropertiesId, lnX, lnZ;
        int lnCount = goBoard.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (goBoard[i] < 1) continue;
            lnUnitPropertiesId = goBoard[i];
            CoordinatesOf(i, out lnX, out lnZ);
            loList.Add(new DistributionBoardUnitCoordinates(lnUnitPropertiesId, Id, lnX, lnZ));
        }

        return loList;
    }
}