using System;

[Serializable]
public class DistributionBoardUnitCoordinates
{
    /// <summary>
    /// Id del objeto
    /// </summary>
    public int Id;

    /// <summary>
    /// Id de la unidad
    /// </summary>
    public int UnitPropertiesId;

    /// <summary>
    /// Id del tablero de distribución
    /// </summary>
    public int DistributionBoardId;

    /// <summary>
    /// Coordenada en X.
    /// </summary>
    public int X;

    /// <summary>
    /// Coordenada en Z.
    /// </summary>
    public int Z;

    public DistributionBoardUnitCoordinates() { }
    public DistributionBoardUnitCoordinates(int lnUnitPropertiesId, int lnDistributionBoardId, int lnX, int lnZ) 
    {
        UnitPropertiesId = lnUnitPropertiesId;
        DistributionBoardId = lnDistributionBoardId;
        X = lnX;
        Z = lnZ;
    }
}