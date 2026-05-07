using System;
using System.Collections.Generic;

[Serializable]
public class DistributionBoard
{
    /// <summary>
    /// Id de nuestro tablero de distribución.
    /// </summary>
    public int DistributionBoardId = 0;

    /// <summary>
    /// Nombre de nuestro tablero de distribución.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Largo de nuestro tablero.
    /// Nota: Debe dejarse publico para cargarlo del JSON de playerprefs.
    /// </summary>
    public int Lenght = 0;

    /// <summary>
    /// Nuestro tablero de distribución de unidades, 0 indica que no hay nada y un numero indica el id de la unidad.
    /// Nota: Debe dejarse publico para cargarlo del JSON de playerprefs.
    /// </summary>
    public int[] Board = new int[0];

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lnId"></param>
    /// <param name="lnWidth"></param>
    /// <param name="lnLenght"></param>
    public DistributionBoard(int lnId, int lnWidth, int lnLenght)
    {
        DistributionBoardId = lnId;
        Lenght = lnLenght;
        Board = new int[lnWidth * lnLenght];
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
        return Board[lnIndex];
    }

    /// <summary>
    /// Nos da el indice de las coordenadas dentro del array unidimensional.
    /// </summary>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    /// <returns></returns>
    private int IndexOf(int lnX, int lnZ)
    {
        return (lnX * Lenght) + lnZ;
    }

    /// <summary>
    /// Nos da las cooredenadas del index dado.
    /// </summary>
    /// <param name="lnIndex"></param>
    /// <param name="lnX"></param>
    /// <param name="lnZ"></param>
    private void CoordinatesOf(int lnIndex, out int lnX, out int lnZ)
    {
        lnX = UnityEngine.Mathf.FloorToInt((float)lnIndex / Lenght);
        lnZ = lnIndex - (Lenght * lnX);
    }

    /// <summary>
    /// Nos indica si el id se encuentra dentro del array.
    /// </summary>
    /// <param name="lnUnitStoredPropertiesId"></param>
    /// <returns></returns>
    public bool Contains(int lnUnitStoredPropertiesId)
    {
        if (Board == null) return false;
        foreach (int lnCellId in Board)
        {
            if (lnCellId == lnUnitStoredPropertiesId) return true;
        }
        return false;
    }

    /// <summary>
    /// Se llama para remover una unidad del mapa de distribución con base en el id dado.
    /// </summary>
    /// <param name="lnUnitStoredPropertiesId"></param>
    public void AddUnit(int lnUnitStoredPropertiesId, int lnX, int lnZ)
    {
        Board[IndexOf(lnX, lnZ)] = lnUnitStoredPropertiesId;
    }

    /// <summary>
    /// Se llama para remover una unidad del mapa de distribución con base en el id dado.
    /// </summary>
    /// <param name="lnUnitStoredPropertiesId"></param>
    public bool RemoveUnit(int lnUnitStoredPropertiesId)
    {
        int lnCount = Board.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (Board[i] != lnUnitStoredPropertiesId) continue;
            Board[i] = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Se llama para remover una unidad del mapa de distribución con base en el id dado.
    /// </summary>
    /// <param name="lnUnitStoredPropertiesId"></param>
    public bool RemoveUnit(int lnUnitStoredPropertiesId, out int lnX, out int lnZ)
    {
        int lnCount = Board.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (Board[i] != lnUnitStoredPropertiesId) continue;
            Board[i] = 0;
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
        int lnCount = Board.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (Board[i] > 0) lnCountUnits++;
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

        int lnUnitStoredPropertiesId, lnX, lnZ;
        int lnCount = Board.Length;
        for (int i = 0; i < lnCount; i++)
        {
            if (Board[i] < 1) continue;
            lnUnitStoredPropertiesId = Board[i];
            CoordinatesOf(i, out lnX, out lnZ);
            loList.Add(new DistributionBoardUnitCoordinates(lnUnitStoredPropertiesId, DistributionBoardId, lnX, lnZ));
        }

        return loList;
    }
}