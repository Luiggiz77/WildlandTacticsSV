using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabMissiles : PlayablePrefab
{
    [Tooltip("Tiempo en el que debe realizarse el movimiento del misil.")]
    public float time = 1.0f;

    [Tooltip("Curva para la posicion en X.")]
    public AnimationCurve curveSide = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    [Tooltip("Curva para la posicion en Y.")]
    public AnimationCurve curveUp = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);

    [Tooltip("Curva para la posicion en Z.")]
    public AnimationCurve curveForward = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Elemento que se usa de misil (prefab).")]
    public VisualEffect missile;

    /// <summary>
    /// Tiempo de nuestro tiro.
    /// </summary>
    protected float gnTime = 0.0f;

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

        //<< Creamos y configuramos los datos de los missiles necesarios.
        int lnHashCodeMissile = missile.GetHashCode();
        List<VisualEffect> loMissiles = new List<VisualEffect>();

        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        int lnCells = loCells.Length;
        float lnCellDistance;
        float[] loYAngles = new float[lnCells];
        float[] loDistances = new float[lnCells];
        Vector3 loCellPosition;
        Vector3 loDirection;
        Vector3 loParentDirection = Vector3.one;
        VisualEffect loMissle;

        //<< Obtenemos la direccion del padre.
        for (int i = 0; i < lnCells; i++)
        {
            loCellPosition = loCells[i].Vector;
            loDirection = loCellPosition - transform.position;
            loParentDirection.x *= loDirection.x;
            loParentDirection.y *= loDirection.y;
            loParentDirection.z *= loDirection.z;
        }

        transform.rotation = Quaternion.LookRotation(loParentDirection, Vector3.up);

        //<< Obtenemos las direcciones sobre el eje Y de cada misil.
        for (int i = 0; i < lnCells; i++)
        {
            loCellPosition = loCells[i].Vector;
            loDirection = loCellPosition - transform.position;
            loYAngles[i] = Vector3.SignedAngle(transform.forward, loDirection, Vector3.up);
            loDistances[i] = lnCellDistance = Vector3.Distance(transform.position, loCellPosition);
            loMissle = GameManager.Spawn(lnHashCodeMissile, missile);
            loMissle.transform.SetParent(transform);
            loMissle.transform.localPosition = Vector3.zero;
            loMissle.transform.localRotation = Quaternion.identity;
            loMissle.Play();
            loMissiles.Add(loMissle);
        }

        goMicroAnimationMissile.SetTime(gnTime);
        goMicroAnimationMissile.SetAsAAnimateToB();

        //<< Desactivamos el misil original ya que usaremos los nuevos misiles.
        missile.gameObject.SetActive(false);

        //<< Asignamos la posicion con base en el tiempo.
        float lnCurveZ, lnCurveY;
        Vector3 loCurvesPosition = Vector3.zero;
        Vector3 loCurrentPosition;
        VisualEffect loMissile;
        float lnDelta;
        do
        {
            lnDelta = goMicroAnimationMissile.GetDelta();
            loCurvesPosition.x = curveSide.Evaluate(lnDelta);
            lnCurveY = curveUp.Evaluate(lnDelta);
            lnCurveZ = curveForward.Evaluate(lnDelta);

            for (int i = 0; i < lnCells; i++)
            {
                loMissile = loMissiles[i];
                loCurvesPosition.z = lnCurveZ * loDistances[i];
                loCurvesPosition.y = Mathf.Lerp(transform.position.y, loCells[i].Vector.y, lnDelta) + lnCurveY;
                loCurrentPosition = Quaternion.Euler(0.0f, loYAngles[i], 0.0f) * loCurvesPosition;

                loDirection = loCurrentPosition - loMissile.transform.localPosition;
                if (loDirection != Vector3.zero) loMissile.transform.localRotation = Quaternion.LookRotation(loDirection, Vector3.up);
                loMissile.transform.localPosition = loCurrentPosition;
            }

            yield return loWaitForEndOfFrame;
            goMicroAnimationMissile.Update();
        }
        while (goMicroAnimationMissile.IsAnimating());

        //<< Ya que terminó el flujo de los misiles los desactivamos.
        for (int i = 0; i < lnCells; i++)
        {
            loMissiles[i].Stop();
            loMissiles[i].gameObject.SetActive(false);
        }

        Stop();

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
        missile.transform.localPosition = Vector3.zero;
        missile.transform.localRotation = Quaternion.identity;
        missile.gameObject.SetActive(true);
    }
}