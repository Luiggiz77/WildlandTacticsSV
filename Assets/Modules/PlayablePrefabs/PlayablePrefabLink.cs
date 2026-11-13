using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabLink : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el enlace.")]
    public float time = 1.0f;

    [Tooltip("Curva para la posicion en X.")]
    public AnimationCurve curveSide = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    [Tooltip("Curva para la posicion en Y.")]
    public AnimationCurve curveUp = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    [Tooltip("Curva para la posicion en Z.")]
    public AnimationCurve curveForward = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Elemento que se usa de conector base.")]
    public Transform link;

    [Tooltip("Elemento que se usa de conectores.")]
    public Transform[] links;

    /// <summary>
    /// Tiempo de nuestro tiro.
    /// </summary>
    private float gnTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    protected MicroAnimation goMicroAnimationLink = new MicroAnimation();

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
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        float lnDistance = Vector3.Distance(transform.position, loCells[0].Vector);
        goMicroAnimationLink.SetSpeed(lnDistance / gnTime);
        goMicroAnimationLink.SetAsAAnimateToB();

        link.localPosition = Vector3.zero;
        link.localRotation = Quaternion.identity;
        link.gameObject.SetActive(true);

        //<< Asignamos la posicion con base en el tiempo.
        Vector3 loCurrentPosition;
        float lnDelta;
        float lnStep = 1.0f / links.Length;
        do
        {
            lnDelta = goMicroAnimationLink.GetDelta();
            loCurrentPosition.x = curveSide.Evaluate(lnDelta);
            loCurrentPosition.y = curveUp.Evaluate(lnDelta);
            loCurrentPosition.z = curveForward.Evaluate(lnDelta) * lnDistance * lnDelta;
            link.localPosition = loCurrentPosition;

            yield return loWaitForEndOfFrame;
            goMicroAnimationLink.Update();
        }
        while (goMicroAnimationLink.IsAnimating());

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
        if (goMicroAnimationLink.IsAnimating()) return true;
        return false;
    }

    /// <summary>
    /// Establece un valor. En este caso el tiempo del tiro.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        if (!float.TryParse(loValue as string, out float lnTime)) return;
        gnTime = lnTime;
    }

    /// <summary>
    /// Indica que debemos terminar con el prefab.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Stop()
    {
        gameObject.SetActive(false);
        link.gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
