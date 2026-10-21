/// <summary>
/// Indica el tipo de area.
/// </summary>
public enum GameplayEffectAreaType : int
{
    /// <summary>
    /// Indica que no hay tipo de area.
    /// </summary>
    None = 0,
    /// <summary>
    /// Indica que es en todo el tablero.
    /// </summary>
    Board = 1,
    /// <summary>
    /// Indica que es area en linea recta.
    /// </summary>
    Line = 2,
    /// <summary>
    /// Indica que es area radial.
    /// </summary>
    Radial = 3,
    /// <summary>
    /// Indica que es en cruz.
    /// </summary>
    Cross = 4,
    /// <summary>
    /// Indica que es en cadena de celdas que estén ocupadas y se ejecuta sobre los ejes (nunca en diagonal).
    /// </summary>
    Chain = 5,
    /// <summary>
    /// Indica que solo es una celda.
    /// </summary>
    Single = 6,
    /// <summary>
    /// Indica que es la celda en la que se encuentra la unidad que llama el efecto.
    /// </summary>
    Self = 7,
    /// <summary>
    /// Indica que el area es en forma de X.
    /// </summary>
    X = 8,
    /// <summary>
    /// Indica que el area es en forma perpendicular a la dirección de la unidad.
    /// </summary>
    Perpendicular = 9,

    /// <summary>
    /// Indica que el area es en forma de T a la dirección de la unidad.
    /// </summary>
    T_Normal = 10,

    /// <summary>
    /// Indica que el area es en forma de T invertida a la dirección de la unidad.
    /// </summary>
    T_Inverted = 11,

    /// <summary>
    /// Indica que el area es en forma de T a la dirección de la unidad. Sin la celda que converge.
    /// </summary>
    T_Normal_Hollow = 12,

    /// <summary>
    /// Indica que el area es en forma de T invertida a la dirección de la unidad. Sin la celda que converge.
    /// </summary>
    T_Inverted_Hollow = 13,

    /// <summary>
    /// Indica que es en cruz pero sin incluir el centro de la cruz.
    /// </summary>
    CrossHollow = 14,

    /// <summary>
    /// Indica que el area es en forma de X sin incluir el centro.
    /// </summary>
    XHollow = 15,

    /// <summary>
    /// Indica que es una cruz en una posicion aleatoria en el mapa.
    /// </summary>
    RandomPositionCross = 100,

    /// <summary>
    /// Unidad mas lejana dentro del area de rango dado.
    /// </summary>
    FurthestUnit = 101,

    /// <summary>
    /// Unidad mas cercana dentro del area de rango dado.
    /// </summary>
    NearestUnit = 102,

    /// <summary>
    /// Indica que es una posicion aleatoria en el mapa que esté vacia.
    /// </summary>
    RandomEmptyPosition = 103,

    /// <summary>
    /// Indica que es una posicion aleatoria alrededor de quien llama o del objetivo.
    /// </summary>
    RandomArroundTargetOrCaller = 104,
}