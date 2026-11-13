using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabMissile : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el movimiento del misil.")]
    public float time = 1.0f;

    [Tooltip("Curva para la posicion en X.")]
    public AnimationCurve curveSide = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    [Tooltip("Curva para la posicion en Y.")]
    public AnimationCurve curveUp = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    [Tooltip("Curva para la posicion en Z.")]
    public AnimationCurve curveForward = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Elemento que se usa de misil.")]
    public Transform missile;

    /// <summary>
    /// Tiempo de nuestro tiro.
    /// </summary>
    private float gnTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    protected MicroAnimation goMicroAnimationMissile = new MicroAnimation();

    /// <summary>
    /// OnEnable
    /// </summary>
    public void OnEnable()
    {
        gnTime = time;
    }

    /// <summary>
    /// Reproducimos la animacion.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
    }

    /// <summary>
    /// Reproducimos la animacion en modo corrutina.
    /// </summary>
    /// <param name="loPosition"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (loCells.Length == 0) yield break;
        Vector3 loPosition = loCells[0].Vector;
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        Vector3 loDirection = loPosition - transform.position;
        transform.rotation = Quaternion.LookRotation(loDirection, Vector3.up);

        float lnDistance = Vector3.Distance(transform.position, loPosition);
        goMicroAnimationMissile.SetSpeed(lnDistance / gnTime);
        goMicroAnimationMissile.SetAsAAnimateToB();

        Preview();

        //<< Asignamos la posicion con base en el tiempo.
        Vector3 loCurrentPosition;
        float lnDelta;
        do
        {
            lnDelta = goMicroAnimationMissile.GetDelta();
            loCurrentPosition.x = curveSide.Evaluate(lnDelta);
            loCurrentPosition.y = curveUp.Evaluate(lnDelta);
            loCurrentPosition.z = curveForward.Evaluate(lnDelta) * lnDistance * lnDelta;
            loDirection = loCurrentPosition - missile.localPosition;
            if(loDirection != Vector3.zero) missile.localRotation = Quaternion.LookRotation(loDirection, Vector3.up);
            missile.localPosition = loCurrentPosition;

            yield return loWaitForEndOfFrame;
            goMicroAnimationMissile.Update();
        }
        while (goMicroAnimationMissile.IsAnimating());

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
        for (int i = 0; i < lnCells; i++) yield return PlayCoroutine(loValue, lnDistanceMax, loCells[i]);
    }

    public override bool Ended()
    {
        if (!gameObject.activeSelf) return true;
        if (goMicroAnimationMissile.IsAnimating()) return true;
        return false;
    }

    /// <summary>
    /// Establece un valor. En este caso el tiempo del tiro.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        if (!float.TryParse((string)loValue, out float lnTime)) return;
        gnTime = lnTime;
    }

    /// <summary>
    /// Indica que debemos terminar con el prefab.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Stop()
    {
        gameObject.SetActive(false);
        missile.gameObject.SetActive(false);
    }

    /// <summary>
    /// Prevista del missil.
    /// </summary>
    public override void Preview()
    {
        missile.localPosition = Vector3.zero;
        missile.localRotation = Quaternion.identity;
        missile.gameObject.SetActive(true);
    }
}
