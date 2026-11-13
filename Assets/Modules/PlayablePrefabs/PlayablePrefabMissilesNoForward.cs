using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabMissilesNoForward : PlayablePrefabMissiles
{
    /// <summary>
    /// Reproducimos la animacion en modo corrutina.
    /// </summary>
    /// <param name="loPosition"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (loCells.Length == 0)
        {
            Stop();
            yield break;
        }

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

        for (int i = 0; i < lnCells; i++)
        {
            loCellPosition = loCells[i].Vector;
            loDirection = loCellPosition - transform.position;
            loYAngles[i] = Vector3.SignedAngle(transform.forward, loDirection, Vector3.up);
            loDistances[i] = lnCellDistance = Vector3.Distance(transform.position, loCellPosition);
            
            loMissle = GameManager.Spawn(lnHashCodeMissile, missile);
            loMissle.transform.SetParent(transform);
            loMissle.transform.localScale = missile.transform.localScale;
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
            loCurvesPosition.x = 0.0f;
            lnCurveY = curveUp.Evaluate(lnDelta);
            lnCurveZ = curveForward.Evaluate(lnDelta);

            for (int i = 0; i < lnCells; i++)
            {
                loMissile = loMissiles[i];
                loCurvesPosition.z = lnCurveZ * loDistances[i];
                loCurvesPosition.y = Mathf.Lerp(transform.position.y, loCells[i].Vector.y, lnDelta) + lnCurveY;
                loCurrentPosition = Quaternion.Euler(0.0f, loYAngles[i], 0.0f) * loCurvesPosition;

                loMissile.transform.localPosition = loCurrentPosition;
                loDirection = Camera.main.transform.position - loCurrentPosition;
                loMissile.transform.rotation = Quaternion.LookRotation(loDirection, Vector3.up);
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
}