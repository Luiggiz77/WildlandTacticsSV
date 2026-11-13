using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayablePrefafUIEffect : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el efecto.")]
    public float time = 1.0f;

    [Tooltip("Es el texto que nos indica el valor asignado del efecto.")]
    public TMP_Text text;

    [Tooltip("Para poder cambiar el alpha de los elementos.")]
    public CanvasGroup canvasGroup;

    [Tooltip("Curva para la animacion de la escala.")]
    public AnimationCurve curveScale = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Curva para la animacion de la traslación.")]
    public AnimationCurve curveTranslation = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Curva para la animacion de la transparencia.")]
    public AnimationCurve curveAlpha = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    /// <summary>
    /// Tiempo para reproducir.
    /// </summary>
    private float gnTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// OnEnable
    /// </summary>
    public void OnEnable()
    {
        gnTime = time;
    }

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (loCells.Length == 0) yield break;
        Vector3 loPosition = loCells[0].Vector;
        Vector3 loStartPosition = transform.position;
        text.gameObject.SetActive(true);
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        goMicroAnimation.SetTime(gnTime);
        goMicroAnimation.SetAsAAnimateToB();
        float lnDelta;
        float lnDeltaScale;
        float lnDeltaAlpha;
        float lnDeltaTranslation;
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            lnDeltaScale = curveScale.Evaluate(lnDelta);
            lnDeltaAlpha = curveAlpha.Evaluate(lnDelta);
            lnDeltaTranslation = curveTranslation.Evaluate(lnDelta);

            canvasGroup.transform.position = Vector3.Lerp(loStartPosition, loPosition, lnDeltaTranslation);
            canvasGroup.transform.localScale = Vector3.one * lnDeltaScale;
            canvasGroup.alpha = lnDeltaAlpha;

            yield return loWaitForEndOfFrame;
            goMicroAnimation.Update();
        }
        while (goMicroAnimation.IsAnimating());

        yield break;
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (!goMicroAnimation.IsAnimating()) return true;
        return false;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield return PlayCoroutine(loValue, lnDistanceMax, loCells);
    }

    public override void SetValue(object loValue)
    {
        text.text = (string)loValue;
    }

    public override void Stop()
    {
        gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
