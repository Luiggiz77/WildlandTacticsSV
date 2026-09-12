using UnityEngine;
using UnityEngine.EventSystems;

public class UIFloating : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Tooltip("Nuestro RectTransform de todo el menu")]
    public RectTransform rectTransform;
    [Tooltip("Canvas al que pertenecemos")]
    public Canvas canvas;
    [Tooltip("RectTransform del Canvas al que pertenecemos")]
    public RectTransform panelRectTransform;
    [Tooltip("Se usa para activar o desactivar el raycast sobre todo el elemento y no activar botones u otros elementos del menú a la hora de modificar su posición.")]
    public CanvasGroup handles;
    [Tooltip("Esquina superior izquierda para mover.")]
    public RectTransform[] dragHandles;
    [Tooltip("Esquina inferior derecha para redimensionar.")]
    public RectTransform[] resizeHandles;

    [Tooltip("Límites de Tamaño minimo")]
    public Vector2 minSize = new Vector2(200f, 150f);
    [Tooltip("Límites de Tamaño maximo")]
    public Vector2 maxSize = new Vector2(1920f, 1080f);

    [Tooltip("Si está activo, la ventana no podrá salir completamente de los bordes de la pantalla.")]
    public bool restrictToScreen = true;
    [Tooltip("Margen mínimo visible en pixeles cuando choca con un borde.")]
    public float screenEdgePadding = 50f;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        
    }

    /// <summary>
    /// OnPointerDown
    /// </summary>
    /// <param name="loEventData"></param>
    public void OnPointerDown(PointerEventData loEventData)
    {
        rectTransform.SetAsLastSibling();
    }

    /// <summary>
    /// OnDrag
    /// </summary>
    /// <param name="loEventData"></param>
    public void OnDrag(PointerEventData loEventData)
    {
        if (canvas == null) return;

        if (resizeHandles != null && RectangleContainsScreenPoint(resizeHandles, loEventData.position, canvas.worldCamera)) Resize(loEventData);
        else if (dragHandles == null || RectangleContainsScreenPoint(dragHandles, loEventData.position, canvas.worldCamera)) Move(loEventData);
    }

    /// <summary>
    /// Modo loop.
    /// </summary>
    /// <param name="loRectTransforms"></param>
    /// <param name="loScreenPoint"></param>
    /// <param name="loCamera"></param>
    /// <returns></returns>
    private static bool RectangleContainsScreenPoint(RectTransform[] loRectTransforms, Vector2 loScreenPoint, Camera loCamera)
    {
        foreach (RectTransform loItem in loRectTransforms)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(loItem, loScreenPoint, loCamera)) return true;
        }
        return false;
    }

    /// <summary>
    /// Para cuando movemos
    /// </summary>
    /// <param name="loPointerEventData"></param>
    private void Move(PointerEventData loPointerEventData)
    {
        Vector2 loMoveDelta;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
        {
            loMoveDelta = loPointerEventData.delta;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, loPointerEventData.position, canvas.worldCamera, out var loLocalPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, loPointerEventData.position - loPointerEventData.delta, canvas.worldCamera, out var loOldLocalPoint);
            loMoveDelta = loLocalPoint - loOldLocalPoint;
        }

        rectTransform.anchoredPosition += loMoveDelta;
        if (restrictToScreen) ApplyScreenConstraints();
    }

    /// <summary>
    /// Para cuando cambiamos de tamaño.
    /// </summary>
    /// <param name="loPointerEventData"></param>
    private void Resize(PointerEventData loPointerEventData)
    {
        float lnScaleFactor = canvas.scaleFactor;
        Vector2 loPointerDelta = loPointerEventData.delta / lnScaleFactor;
        Vector2 lnSizeDelta = rectTransform.sizeDelta;
        lnSizeDelta += new Vector2(loPointerDelta.x, -loPointerDelta.y);

        //<< Limitamos tamaño mínimo y máximo
        lnSizeDelta.x = Mathf.Clamp(lnSizeDelta.x, minSize.x, maxSize.x);
        lnSizeDelta.y = Mathf.Clamp(lnSizeDelta.y, minSize.y, maxSize.y);

        rectTransform.sizeDelta = lnSizeDelta;
        if (restrictToScreen) ApplyScreenConstraints();
    }

    /// <summary>
    /// Evitamos que se salga de la pantalla.
    /// </summary>
    private void ApplyScreenConstraints()
    {
        if (panelRectTransform == null) return;

        Vector2 loCanvasSize = panelRectTransform.rect.size;
        Vector2 loCurrentSize = rectTransform.rect.size;
        Vector2 loPivot = rectTransform.pivot;
        Vector2 loAnchoredPos = rectTransform.anchoredPosition;

        //<< Calculamos los límites del canvas considerando el pivote de la ventana
        float lnMinPosX = (loCanvasSize.x * -0.5f) + (loCurrentSize.x * loPivot.x) + screenEdgePadding - (loCanvasSize.x * 0.5f) * (rectTransform.anchorMax.x - 0.5f); // Simplificado para anclas centradas

        float lnHalfCanvasX = loCanvasSize.x * 0.5f;
        float lnHalfCanvasY = loCanvasSize.y * 0.5f;

        //<< Calculamos los límites min/max permitidos para anchoredPosition (con anclas al centro del Canvas).
        float lnMinX = -lnHalfCanvasX + (loCurrentSize.x * loPivot.x) - screenEdgePadding;
        float lnMaxX = lnHalfCanvasX - (loCurrentSize.x * (1 - loPivot.x)) + screenEdgePadding;
        float lnMinY = -lnHalfCanvasY + (loCurrentSize.y * loPivot.y) - screenEdgePadding;
        float lnMaxY = lnHalfCanvasY - (loCurrentSize.y * (1 - loPivot.y)) + screenEdgePadding;

        //<< Restringir posición
        loAnchoredPos.x = Mathf.Clamp(loAnchoredPos.x, lnMinX, lnMaxX);
        loAnchoredPos.y = Mathf.Clamp(loAnchoredPos.y, lnMinY, lnMaxY);

        rectTransform.anchoredPosition = loAnchoredPos;
    }
}