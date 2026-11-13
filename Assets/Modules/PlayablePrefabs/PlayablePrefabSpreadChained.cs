using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayablePrefabSpreadChained : PlayablePrefab
{
    [Tooltip("Indica que el tiempo dado se usa para el salto entre objetivos.")]
    public bool spreadTimePerTarget = true;

    [Tooltip("Tiempo en el que debe realizarse el spread ya sea por objetivo o para todos los objetivos.")]
    public float spreadTime = 1.0f;

    [Tooltip("Elemento que se instancia N veces para conectar las unidades.")]
    public PlayablePrefabChild link;

    /// <summary>
    /// Tiempo para esparcir los elementos en cruz.
    /// </summary>
    private float gnSpreadTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// Para tener las referencias de todos los elementos.
    /// </summary>
    private Dictionary<int, BoardCell<BoardUnit>[]> goAreaCells = new Dictionary<int, BoardCell<BoardUnit>[]>();

    /// <summary>
    /// Particulas instanciadas que van iniciando su camino al objetivo.
    /// </summary>
    private List<PlayablePrefabChild> goFlow = new List<PlayablePrefabChild>();

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
        //<< Agregamos a la lista.
        BoardCell<BoardUnit>[] loClone = new BoardCell<BoardUnit>[lnCells];
        Array.Copy(loCells, loClone, lnCells);
        goAreaCells.Add(lnDistanceCurrent, loClone);

        //<< Si es el inicio continuamos.
        if (lnDistanceCurrent == 0) yield break;

        //<< Obtenemos la lista previa.
        BoardCell<BoardUnit>[] loLast = goAreaCells[lnDistanceCurrent - 1];

        //<< Obtenemos la lista actual.
        BoardCell<BoardUnit>[] loCurrent = goAreaCells[lnDistanceCurrent];

        int lnHashCodePrefab = link.GetHashCode();

        //<< Determinamos de que celdas a que celdas se mueve la cadena y creamos los elementos que se moverán.
        PlayablePrefabChild loInstantiated;
        Facing loFacing;
        List<PlayablePrefabChild> loChilds = new List<PlayablePrefabChild>();
        foreach (BoardCell<BoardUnit> loItem in loLast)
        {
            //<< Determinamos cuantas creaciones debemos hacer por vecindad.
            foreach (BoardCell<BoardUnit> loElement in loCurrent)
            {
                if (!loItem.IsNeighbor(loElement, out loFacing)) continue;

                //<< Instanciamos un elemento.
                loInstantiated = GameManager.Spawn(lnHashCodePrefab, link);

                //<< Asignamos la posición de inicio.
                loInstantiated.transform.position = loItem.Vector;

                //<< Asignamos la dirección.
                loInstantiated.transform.rotation = loFacing.ToQuaternion();

                //<< Agregamos a la lista de elemento que se moverán.
                goFlow.Add(loInstantiated);
                loChilds.Add(loInstantiated);
            }
        }

        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();

        if(spreadTimePerTarget) goMicroAnimation.SetTime(gnSpreadTime);
        else goMicroAnimation.SetTime(gnSpreadTime / lnDistanceMax);
        goMicroAnimation.SetAsAAnimateToB();
        float lnDelta;
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            foreach (PlayablePrefabChild loItem in loChilds) loItem.SetDelta(lnDelta);
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
        foreach (PlayablePrefabChild loItem in goFlow) loItem.Stop();
        goFlow.Clear();

        gameObject.SetActive(false);
        goAreaCells.Clear();
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
