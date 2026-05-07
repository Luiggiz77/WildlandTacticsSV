using UnityEngine;

public class DistributionBoardUnitCoordinates
{
    [Tooltip("Id del objeto")]
    public int Id;

    [Tooltip("Id de la unidad")]
    public int UnitPropertiesId;

    [Tooltip("Id del tablero de distribución")]
    public int DistributionBoardId;

    [Tooltip("Coordenada en X.")]
    public int X;

    [Tooltip("Coordenada en Z.")]
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