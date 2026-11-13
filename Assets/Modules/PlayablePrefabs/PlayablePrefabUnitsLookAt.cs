using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabUnitsLookAt : PlayablePrefabMissile
{
    [Tooltip("Nos indica cuantos segundos las unidades firjarán la mirada en este GameObject.")]
    public float timeLookAt = 5.0f;

    [Tooltip("Nos indica en cuantos segundos las unidades girarán la cabeza para mirar este GameObject.")]
    public float timeFaceTurn = 0.5f;

    /// <summary>
    /// Microanimación para el lookat.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        goMicroAnimation.SetTime(timeFaceTurn);
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        if (goMicroAnimation.IsAnimating()) goMicroAnimation.Update();
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        //<< Esperamos que el misíl comience su recorrido.
        while (!goMicroAnimationMissile.IsAnimating()) yield return loWaitForEndOfFrame;

        //<< Esperamos que el misil llegue hasta la mitad de su camino.
        while(goMicroAnimationMissile.IsAnimating() && goMicroAnimationMissile.GetDelta() < 0.5f) yield return loWaitForEndOfFrame;

        //<< Para cada unidad de la celda asignamos la función que debe llamarse sobre el IK.
        BoardCell<BoardUnit> loItem;
        for (int i = 0; i < lnCells; i++)
        {
            loItem = loCells[i];

            //<< Si no hay unidades continuamos.
            if (loItem.Data.Count == 0) continue;
            foreach (BoardUnit loBoardUnit in loItem.Data)
            {
                if (loBoardUnit.UnitInstance == null) continue;
                loBoardUnit.UnitInstance.OnUnitAnimatorIK = OnUnitAnimatorIK;
            }
        }

        //<< Iniciamos animación de lookat.
        goMicroAnimation.SetAsAAnimateToB();

        //<< Esperamos la cantidad de segundos dada.
        yield return new WaitForSeconds(timeLookAt);

        //<< Removemos la mirada del objeto.
        goMicroAnimation.AnimateToA();

        //<< Esperamos ha que termine la animación.
        while (goMicroAnimation.IsAnimating()) yield return loWaitForEndOfFrame;

        //<< Para cada unidad de la celda removemos la función que debe llamarse sobre el IK.
        for (int i = 0; i < lnCells; i++)
        {
            loItem = loCells[i];

            //<< Si no hay unidades continuamos.
            if (loItem.Data.Count == 0) continue;
            foreach (BoardUnit loBoardUnit in loItem.Data)
            {
                if (loBoardUnit.UnitInstance == null) continue;
                loBoardUnit.UnitInstance.OnUnitAnimatorIK = null;
            }
        }

        yield break;
    }

    /// <summary>
    /// Se llama desde cada unidad para procesar el look at.
    /// </summary>
    /// <param name="loUnitInstance"></param>
    /// <param name="lnLayerIndex"></param>
    private void OnUnitAnimatorIK(UnitInstance loUnitInstance, int lnLayerIndex)
    {
        loUnitInstance.animator.SetLookAtWeight(goMicroAnimation.GetDelta());
        loUnitInstance.animator.SetLookAtPosition(transform.position);
    }

    public override void SetValue(object loValue)
    {
    }

    public override void Stop()
    {
        gameObject.SetActive(false);
        base.Stop();
    }
}