using UnityEngine;

public struct StoredVector
{
    /// <summary>
    /// Indica el tipo de modificación, ya sea "p" Position, "s" Scale, "r" Rotation
    /// </summary>
    public char T;
    /// <summary>
    /// Indica el valor normalizado para posicionar el vector modificado.
    /// </summary>
    public float N;
    /// <summary>
    /// Valores del vector que modifica una de las propiedades.
    /// </summary>
    public Vector3 V;
}
