using UnityEngine;

public static partial class TransformExtensions
{
    /// <summary>
    /// Busca el transform mas proximo con el nombre que contenga el string "lcName" dado, si por ejemplo hay dos con el mismo nombre busca el mas proximo.
    /// </summary>
    /// <param name="loParent"></param>
    /// <param name="lcName"></param>
    /// <returns></returns>
    public static Transform FindNearestContains(this Transform loParent, string lcName)
    {
        Transform loChild;
        int lnChilds = loParent.childCount;
        for (int i = 0; i < lnChilds; i++)
        {
            loChild = loParent.GetChild(i);
            if (loChild.name.ToLower().Contains(lcName)) return loChild;
        }

        Transform loDeepChild;
        for (int i = 0; i < lnChilds; i++)
        {
            loChild = loParent.GetChild(i);
            loDeepChild = loChild.FindNearestContains(lcName);
            if (loDeepChild != null) return loDeepChild;
        }

        return null;
    }
}
