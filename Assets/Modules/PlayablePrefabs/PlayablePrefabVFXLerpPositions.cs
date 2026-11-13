using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayablePrefabVFXLerpPositions : PlayablePrefabVFXLerp
{
    [Tooltip("Son posiciones que se actualizan con base en la posicion del objetivo dado.")]
    public Transform[] positions;

    /// <summary>
    /// Reproducimos la animacion en modo corrutina con objetivo.
    /// </summary>
    /// <param name="lcValue"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        if (loCells.Length == 0) yield break;

        Vector3 loPosition = loCells[0].Vector;
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        int lnCount = positions.Length;
        float lnStep = 1.0f / (positions.Length - 1.0f);
        float lnDelta = 0.0f;

        //<< Actualizamos las posiciones.
        Vector3 loPositionLerp;
        for (int i = 0; i < lnCount; i++)
        {
            loPositionLerp = Vector3.Lerp(transform.position, loPosition, lnDelta);
            positions[i].position = loPositionLerp;
            lnDelta += lnStep;
        }

        //<< Esperamos un frame.
        yield return loWaitForEndOfFrame;

        //<< Iniciamos muestra.
        foreach (VisualEffect loItem in vfxs) loItem.Play();
        goMicroAnimation.SetTime(gnTime);
        goMicroAnimation.SetAsAAnimateToB();
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            foreach (VisualEffect loItem in vfxs) loItem.SetFloat(gnPropertieId, lnDelta);
            yield return loWaitForEndOfFrame;
            goMicroAnimation.Update();
        }
        while (goMicroAnimation.IsAnimating());

        yield break;
    }
}
