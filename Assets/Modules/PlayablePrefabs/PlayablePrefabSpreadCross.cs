using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabSpreadCross : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el spread.")]
    public float spreadTime = 1.0f;

    [Tooltip("Elemento sobre Z Positivo.")]
    public ParticleSystem zPos;
    [Tooltip("Elemento sobre Z Negativo.")]
    public ParticleSystem zNeg;
    [Tooltip("Elemento sobre X Positivo.")]
    public ParticleSystem xPos;
    [Tooltip("Elemento sobre X Negativo.")]
    public ParticleSystem xNeg;

    [Tooltip("Curva para la posicion en Y.")]
    public AnimationCurve curveY = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    /// <summary>
    /// Tiempo para esparcir los elementos en cruz.
    /// </summary>
    private float gnSpreadTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// OnEnable
    /// </summary>
    public void OnEnable()
    {
        gnSpreadTime = spreadTime;
    }

    /// <summary>
    /// Reproducimos la animacion.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
    }

    /// <summary>
    /// Reproducimos la animacion en modo corrutina con objetivo.
    /// </summary>
    /// <param name="lcValue"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        yield break;
    }

    /// <summary>
    /// Reproducimos la animacion en modo corrutina con datos de efecto.
    /// </summary>
    /// <param name="lnDistanceMax"></param>
    /// <param name="lnDistanceCurrent"></param>
    /// <param name="loCells"></param>
    /// <param name="lnCells"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        zPos.gameObject.SetActive(true);
        zNeg.gameObject.SetActive(true);
        xPos.gameObject.SetActive(true);
        xNeg.gameObject.SetActive(true);

        zPos.Play();
        zNeg.Play();
        xPos.Play();
        xNeg.Play();

        //<< Si es el inicio continuamos.
        if (lnDistanceCurrent == 0) yield break;
        lnDistanceCurrent--;

        Vector3 loPos = transform.position;

        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        goMicroAnimation.SetTime(gnSpreadTime / lnDistanceMax);
        goMicroAnimation.SetAsAAnimateToB();
        Vector3 loXPos = Vector3.zero, loXNeg = Vector3.zero, loZPos = Vector3.zero, loZNeg = Vector3.zero;
        float lnDistance;
        float lnDelta;
        float lnY;
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            lnDistance = lnDistanceCurrent + lnDelta;
            lnY = curveY.Evaluate(lnDelta);
            loZPos.Set(0.0f, lnY, lnDistance);
            loZNeg.Set(0.0f, lnY, -lnDistance);
            loXPos.Set(lnDistance, lnY, 0.0f);
            loXNeg.Set(-lnDistance, lnY, 0.0f);

            zPos.transform.localPosition = loZPos;
            zNeg.transform.localPosition = loZNeg;
            xPos.transform.localPosition = loXPos;
            xNeg.transform.localPosition = loXNeg;

            yield return loWaitForEndOfFrame;
            goMicroAnimation.Update();
        }
        while (goMicroAnimation.IsAnimating());

        yield break;
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (goMicroAnimation.IsAnimating()) return true;
        return false;
    }

    /// <summary>
    /// Establece un valor. En este caso de tiempo de esparcimiento.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        if (!float.TryParse((string)loValue, out float lnSpreadTime)) return;
        gnSpreadTime = lnSpreadTime;
    }

    /// <summary>
    /// Indica que debemos terminar con el prefab.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Stop()
    {
        gameObject.SetActive(false);
        zPos.gameObject.SetActive(false);
        zNeg.gameObject.SetActive(false);
        xPos.gameObject.SetActive(false);
        xNeg.gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
