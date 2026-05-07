using UnityEngine;

public static partial class RectTransformExtensions
{
    public static void SetLeft(this RectTransform loRectTransform, float lnLeft)
    {
        loRectTransform.offsetMin = new Vector2(lnLeft, loRectTransform.offsetMin.y);
    }

    public static void SetRight(this RectTransform loRectTransform, float lnRight)
    {
        loRectTransform.offsetMax = new Vector2(-lnRight, loRectTransform.offsetMax.y);
    }

    public static void SetTop(this RectTransform loRectTransform, float lnTop)
    {
        loRectTransform.offsetMax = new Vector2(loRectTransform.offsetMax.x, -lnTop);
    }

    public static void SetBottom(this RectTransform loRectTransform, float lnBottom)
    {
        loRectTransform.offsetMin = new Vector2(loRectTransform.offsetMin.x, lnBottom);
    }
}