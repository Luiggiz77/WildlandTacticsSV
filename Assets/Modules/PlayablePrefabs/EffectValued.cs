using TMPro;
using UnityEngine;

public class EffectValued : MonoBehaviour, IAlpha
{
    [Tooltip("Es el texto que nos indica el valor asignado del efecto.")]
    public TMP_Text text;

    [Tooltip("Para poder cambiar el alpha de los elementos.")]
    public CanvasGroup canvasGroup;

    /// <summary>
    /// Se llama para establecer el alpha.
    /// </summary>
    /// <param name="lnAlpha"></param>
    public void SetAlpha(float lnAlpha)
    {
        canvasGroup.alpha = lnAlpha;
    }
}
