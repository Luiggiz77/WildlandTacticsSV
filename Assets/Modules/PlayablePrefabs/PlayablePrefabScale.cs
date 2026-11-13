using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabScale : PlayablePrefab
{
    [Tooltip("Indica el tiempo que se requiere para hacer la animación.")]
    public float scaleTime = 1.0f;
    [Tooltip("Indica la escala inicial.")]
    public Vector3 scaleStart;
    [Tooltip("Indica la escala final.")]
    public Vector3 scaleEnd;

    [Tooltip("Indica la curva de animación de escala.")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    /// <summary>
    /// Se usa para realizar la animación.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        goMicroAnimation.SetTime(scaleTime);
        goMicroAnimation.OnB = Stop;
        goMicroAnimation.SetAsAAnimateToB();
    }

    /// <summary>
    /// Update
    /// </summary>
    public void Update()
    {
        if (goMicroAnimation.OnB == null) return;
        if (!goMicroAnimation.IsAnimating()) return;
        goMicroAnimation.Update();
        transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, curve.Evaluate(goMicroAnimation.GetDelta()));
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (loCells.Length == 0) yield break;
        Vector3 loPosition = loCells[0].Vector;
        transform.position = loPosition;
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        goMicroAnimation.SetTime(scaleTime);
        goMicroAnimation.OnB = null;
        goMicroAnimation.SetAsAAnimateToB();
        do
        {
            transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, curve.Evaluate(goMicroAnimation.GetDelta()));
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
