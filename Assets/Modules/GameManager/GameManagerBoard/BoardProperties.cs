using UnityEngine;

public class BoardProperties
{
    /// <summary>
    /// Ancho del mapa (cuadrantes).
    /// </summary>
    public int boardWidht = 6;

    /// <summary>
    /// Largo del mapa (cuadrantes).
    /// </summary>
    public int boardLenght = 7;

    /// <summary>
    /// Largo del tablero de distribucion de unidades (cuadrantes).
    /// </summary>
    public int distributionBoardLenght = 3;

    /// <summary>
    /// Indica si se deben mostrar los bordes de la malla.
    /// </summary>
    public bool borderLines = true;

    /// <summary>
    /// Indica si se deben mostrar los bordes de la malla.
    /// </summary>
    public bool gridLines = true;

    /// <summary>
    /// Indica si se deben mostrar el checker.
    /// </summary>
    public bool checker = true;

    /// <summary>
    /// Ancho de lineas de la malla en el borde.
    /// </summary>
    public float borderLineWidth = 0.05f;

    /// <summary>
    /// Ancho de lineas de la malla principal.
    /// </summary>
    public float gridLineWidth = 0.01f;

    /// <summary>
    /// Posicion del eje Y sobre el mapa para poner las lineas.
    /// </summary>
    public float gridLineOffsetY = 0.01f;

    /// <summary>
    /// Indica el maximo de unidades que se pueden usar en el tablero de distriución.
    /// </summary>
    public int maxDistributionBoardUnits = 6;

    /// <summary>
    /// Color de las lineas.
    /// </summary>
    public Color gridLineColor = new Color(0, 25, 115, 204);

    /// <summary>
    /// Color del checker.
    /// </summary>
    public Color checkerColor = new Color(0, 25, 115, 204);
}