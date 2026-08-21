using UnityEngine;

public static class FacingExtensions
{
    private static Quaternion XPos = Quaternion.Euler(0.0f, 90.0f, 0.0f);
    private static Quaternion XNeg = Quaternion.Euler(0.0f, -90.0f, 0.0f);
    private static Quaternion ZNeg = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    /// <summary>
    /// Convierete nuestra direccion en tablero a rotación.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <returns></returns>
    public static Quaternion ToQuaternion(this Facing loFacing)
    {
        switch (loFacing)
        {
            case Facing.ZNeg: return ZNeg;
            case Facing.XPos: return XPos;
            case Facing.XNeg: return XNeg;
            default: return Quaternion.identity;
        }
    }

    /// <summary>
    /// Invierte el eje.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFrom"></param>
    /// <returns></returns>
    public static Facing Invert(this Facing loFacing)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return Facing.ZNeg;
            case Facing.ZNeg: return Facing.ZPos;
            case Facing.XPos: return Facing.XNeg;
            case Facing.XNeg: return Facing.XPos;
            default: return Facing.ZPos;
        }
    }

    /// <summary>
    /// Intercambia el eje de Z a X y viceversa manteniendo su signo.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <returns></returns>
    public static Facing Swap(this Facing loFacing)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return Facing.XPos;
            case Facing.ZNeg: return Facing.XNeg;
            case Facing.XPos: return Facing.ZPos;
            case Facing.XNeg: return Facing.ZNeg;
            default: return Facing.ZPos;
        }
    }

    /// <summary>
    /// Indica si es por la izquierda.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFrom"></param>
    /// <returns></returns>
    public static bool FromLeft(this Facing loFacing, Facing loFrom)
    {
        if (loFacing.FromBack(loFrom)) return false;
        if (loFacing.FromFront(loFrom)) return false;

        loFrom = loFrom.Swap();
        return loFacing.FromBack(loFrom);
    }

    /// <summary>
    /// Indica si es por la derecha.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFrom"></param>
    /// <returns></returns>
    public static bool FromRight(this Facing loFacing, Facing loFrom)
    {
        if (loFacing.FromBack(loFrom)) return false;
        if (loFacing.FromFront(loFrom)) return false;

        loFrom = loFrom.Swap();
        loFrom = loFrom.Invert();
        return loFacing.FromBack(loFrom);
    }

    /// <summary>
    /// Indica si es de frente.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFrom"></param>
    /// <returns></returns>
    public static bool FromFront(this Facing loFacing, Facing loFrom)
    {
        return loFacing == loFrom.Invert();
    }

    /// <summary>
    /// Indica si es por la espalda.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFrom"></param>
    /// <returns></returns>
    public static bool FromBack(this Facing loFacing, Facing loFrom)
    {
        return loFacing == loFrom;
    }

    /// <summary>
    /// Influencia nuestra dirección con base en otra dirección.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loInfluence"></param>
    /// <returns></returns>
    public static Facing Influenced(this Facing loFacing, Facing loInfluence)
    {
        switch (loFacing)
        {
            case Facing.ZPos:
                switch (loInfluence)
                {
                    case Facing.ZPos: return Facing.ZNeg;
                    case Facing.ZNeg: return Facing.ZPos;
                    case Facing.XPos: return Facing.XNeg;
                    case Facing.XNeg: return Facing.XPos;
                    default: break;
                }
                break;
            case Facing.ZNeg:
                switch (loInfluence)
                {
                    case Facing.ZPos: return Facing.ZPos;
                    case Facing.ZNeg: return Facing.ZPos;
                    case Facing.XPos: return Facing.XNeg;
                    case Facing.XNeg: return Facing.XPos;
                    default: break;
                }
                break;
            case Facing.XPos: if (loInfluence == Facing.XNeg) return Facing.XNeg; break;
            case Facing.XNeg: if (loInfluence == Facing.XPos) return Facing.XPos; break;
            default: break;
        }

        return loInfluence;
    }

    /// <summary>
    /// Convierte las coordenadas en una dirección.
    /// </summary>
    /// <param name="lnXOrigin"></param>
    /// <param name="lnZOrigin"></param>
    /// <param name="lnXDestination"></param>
    /// <param name="lnZDestination"></param>
    /// <returns></returns>
    public static Facing GetFacing(int lnXOrigin, int lnZOrigin, int lnXDestination, int lnZDestination, Facing loDefault)
    {
        int lnX = lnXDestination - lnXOrigin;
        int lnZ = lnZDestination - lnZOrigin;

        //<< Si no hay desplazamiento mandamos el default.
        if (lnX == lnZ && lnX == 0) return loDefault;

        int lnXAbs = Mathf.Abs(lnX);
        int lnZAbs = Mathf.Abs(lnZ);

        //<< Si son iguales mandamos el default.
        if (lnXAbs == lnZAbs) return loDefault;

        //<< Si X es mayor que Z.
        if (lnXAbs > lnZAbs)
        {
            if (lnX > 0) return Facing.XPos;
            return Facing.XNeg;
        }

        //<< Ya que Z es mayor que X revisamos la dirección del eje Z.
        if (lnZ > 0) return Facing.ZPos;
        return Facing.ZNeg;
    }

    /// <summary>
    /// Nos indica si debemos rotar a 180º.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFacingFinal"></param>
    /// <returns></returns>
    public static bool IsTurn180(this Facing loFacing, Facing loFacingFinal)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return loFacingFinal == Facing.ZNeg;
            case Facing.ZNeg: return loFacingFinal == Facing.ZPos;
            case Facing.XPos: return loFacingFinal == Facing.XNeg;
            case Facing.XNeg: return loFacingFinal == Facing.XPos;
            default: return false;
        }
    }

    /// <summary>
    /// Nos indica si debemos rota 90º positivamente.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFacingFinal"></param>
    /// <returns></returns>
    public static bool IsTurn90Pos(this Facing loFacing, Facing loFacingFinal)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return loFacingFinal == Facing.XPos;
            case Facing.ZNeg: return loFacingFinal == Facing.XNeg;
            case Facing.XPos: return loFacingFinal == Facing.ZNeg;
            case Facing.XNeg: return loFacingFinal == Facing.ZPos;
            default: return false;
        }
    }

    /// <summary>
    /// Nos indica si debemos rota 90º negativamente.
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFacingFinal"></param>
    /// <returns></returns>
    public static bool IsTurn90Neg(this Facing loFacing, Facing loFacingFinal)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return loFacingFinal == Facing.XNeg;
            case Facing.ZNeg: return loFacingFinal == Facing.XPos;
            case Facing.XPos: return loFacingFinal == Facing.ZPos;
            case Facing.XNeg: return loFacingFinal == Facing.ZNeg;
            default: return false;
        }
    }

    /// <summary>
    /// Nos da el forward de la unidad con base en su orientación..
    /// </summary>
    /// <param name="loFacing"></param>
    /// <param name="loFacingFinal"></param>
    /// <returns></returns>
    public static Vector3 Forward(this Facing loFacing)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return Vector3.forward;
            case Facing.ZNeg: return Vector3.back;
            case Facing.XPos: return Vector3.right;
            case Facing.XNeg: return Vector3.left;
            default: return Vector3.zero;
        }
    }

    public static Vector3Int ForwardInt(this Facing loFacing)
    {
        switch (loFacing)
        {
            case Facing.ZPos: return Vector3Int.forward;
            case Facing.ZNeg: return Vector3Int.back;
            case Facing.XPos: return Vector3Int.right;
            case Facing.XNeg: return Vector3Int.left;
            default: return Vector3Int.zero;
        }
    }
}