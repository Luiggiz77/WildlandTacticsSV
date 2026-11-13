using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayablePrefabStatEffect : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el efecto.")]
    public float time = 1.0f;

    [Tooltip("Posicion inicial en Y.")]
    public float offsetYStart = 0.0f;

    [Tooltip("Posicion final en Y apartir de la posicion inicial.")]
    public float offsetYEnd = 1.0f;

    [Tooltip("Escala inicial de los elementos.")]
    public Vector3 scaleStart = Vector3.zero;

    [Tooltip("Escala final de los elementos.")]
    public Vector3 scaleEnd = Vector3.one;

    [Tooltip("Es el texto que nos indica el valor asignado del efecto.")]
    public TMP_Text text;

    [Tooltip("Para poder cambiar el alpha de los elementos.")]
    public CanvasGroup canvasGroup;

    [Tooltip("Curva para la animacion de la escala.")]
    public AnimationCurve curveScale = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Curva para la animacion de la posicion en Y.")]
    public AnimationCurve curveY = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

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
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        goMicroAnimation.SetTime(gnTime);
        goMicroAnimation.SetAsAAnimateToB();
        float lnDelta;
        float lnDeltaScale;
        float lnDeltaAlpha;
        float lnY;
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            lnDeltaScale = curveScale.Evaluate(lnDelta);
            lnDeltaAlpha = curveAlpha.Evaluate(lnDelta);
            lnY = offsetYStart + (offsetYEnd * curveY.Evaluate(lnDelta));

            loPosition.y = lnY;
            canvasGroup.transform.position = loPosition;
            canvasGroup.transform.localScale = scaleStart + (scaleEnd * lnDeltaScale);
            canvasGroup.alpha = lnDeltaAlpha;

            yield return loWaitForEndOfFrame;
            goMicroAnimation.Update();
        }
        while (goMicroAnimation.IsAnimating());

        yield break;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield return PlayCoroutine(loValue, lnDistanceMax, loCells);
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (goMicroAnimation.IsAnimating()) return true;
        return false;
    }

    public override void SetValue(object loValue)
    {
        text.text = loValue as string;
    }

    public override void Stop()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
