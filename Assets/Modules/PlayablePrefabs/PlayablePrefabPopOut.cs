using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabPopOut : PlayablePrefab
{
    [Tooltip("Para animar el elemento.")]
    public CanvasGroup canvasGroup;

    [Tooltip("Curva para la animación de salida.")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Posicion inicial en Y.")]
    public float offsetY = 1.1f;

    [Tooltip("Tiempo para mostrar el efecto.")]
    public float time = 1.0f;

    /// <summary>
    /// Para animar el fadeout.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    private void Awake()
    {
        goMicroAnimation.SetAsA();
        goMicroAnimation.SetTime(time);
        canvasGroup.transform.localPosition = new Vector3(0.0f, offsetY, 0.0f);
    }

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        canvasGroup.gameObject.SetActive(true);
        goMicroAnimation.SetAsAAnimateToB();

        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        while (goMicroAnimation.IsAnimating())
        {
            goMicroAnimation.Update();
            canvasGroup.alpha = curve.Evaluate(goMicroAnimation.GetDelta());
            yield return loWaitForEndOfFrame;
        }

        yield break;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        //<< Creamos tantos elementos como targets.
        int lnHashCode = canvasGroup.GetHashCode();
        List<CanvasGroup> loClones = new List<CanvasGroup>(lnCells);
        CanvasGroup loCanvasGroup;
        Vector3 loOffsetY = new Vector3(0.0f, offsetY, 0.0f);
        for (int i = 0; i < lnCells; i++)
        {
            loCanvasGroup = GameManager.Spawn(lnHashCode, canvasGroup);
            loCanvasGroup.transform.position = loCells[i].Vector + loOffsetY;
            loClones.Add(loCanvasGroup);
        }

        goMicroAnimation.SetAsAAnimateToB();

        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        float lnDelta;
        while (goMicroAnimation.IsAnimating())
        {
            goMicroAnimation.Update();
            lnDelta = curve.Evaluate(goMicroAnimation.GetDelta());
            foreach (CanvasGroup loItem in loClones) loItem.alpha = lnDelta;
            yield return loWaitForEndOfFrame;
        }

        //<< Desactivamos todo.
        foreach (CanvasGroup loItem in loClones) loItem.gameObject.SetActive(false);

        Stop();

        yield break;
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (goMicroAnimation.IsAnimating()) return true;
        return false;
    }

    public override void SetValue(object loValue)
    {
    }

    public override void Stop()
    {
        gameObject.SetActive(false);
        canvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
