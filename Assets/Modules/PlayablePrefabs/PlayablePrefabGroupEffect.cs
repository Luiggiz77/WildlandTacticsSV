using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayablePrefabGroupEffect<T0> : PlayablePrefab where T0 : Component, IAlpha
{
    [Tooltip("Tiempo en el que debe realizarse el efecto.")]
    public float time = 1.0f;

    [Tooltip("Elemento que se instancia N veces para asignar a las unidades.")]
    public T0 element;

    [Tooltip("Escala inicial de los elementos.")]
    public Vector3 scaleStart = Vector3.zero;

    [Tooltip("Escala final de los elementos.")]
    public Vector3 scaleEnd = Vector3.one;

    [Tooltip("Posicion inicial en Y.")]
    public float offsetYStart = 0.0f;

    [Tooltip("Posicion final en Y apartir de la posicion inicial.")]
    public float offsetYEnd = 1.0f;

    [Tooltip("Curva para la animacion de la escala.")]
    public AnimationCurve curveScale = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Curva para la animacion de la posicion en Y.")]
    public AnimationCurve curveY = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [Tooltip("Curva para la animacion de la transparencia.")]
    public AnimationCurve curveAlpha = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    /// <summary>
    /// Tiempo para reproducir.
    /// </summary>
    private float gnTime = 0.0f;

    /// <summary>
    /// Nuestra animación.
    /// </summary>
    private MicroAnimation goMicroAnimation = new MicroAnimation();

    /// <summary>
    /// Lista de elementos instanciados.
    /// </summary>
    private List<T0> goElements = new List<T0>();

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
    /// Reproducimos la animacion en modo corrutina con objetivo.
    /// </summary>
    /// <param name="lcValue"></param>
    /// <returns></returns>
    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        yield break;
    }

    /// <summary>
    /// Se llama cada vez que se crea un elemento.
    /// </summary>
    /// <param name="loElement"></param>
    protected abstract void OnSpawn(T0 loElement);

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
        int lnHashCodePrefab = element.GetHashCode();

        //<< Por cada celda dada generamos un elemento.
        T0 loInstantiated;
        BoardCell<BoardUnit> loBoardCell;
        for (int i = 0; i < lnCells; i++)
        {
            loBoardCell = loCells[i];

            //<< Instanciamos un elemento.
            loInstantiated = GameManager.Spawn(lnHashCodePrefab, element);

            //<< Asignamos la posición de inicio.
            loInstantiated.transform.position = loBoardCell.Vector;

            //<< Avisamos que creamos un elemento.
            OnSpawn(loInstantiated);

            //<< Agregamos a la lista de elemento que se moverán.
            goElements.Add(loInstantiated);
        }

        WaitForEndOfFrame loWaitForEndOfFrame = new WaitForEndOfFrame();
        goMicroAnimation.SetTime(gnTime / lnDistanceMax);
        goMicroAnimation.SetAsAAnimateToB();
        Vector3 loPosition;
        float lnDelta;
        float lnDeltaScale;
        float lnDeltaAlpha;
        float lnY;
        do
        {
            lnDelta = goMicroAnimation.GetDelta();
            lnDeltaScale = curveScale.Evaluate(lnDelta);
            lnDeltaAlpha = curveAlpha.Evaluate(lnDelta);
            lnY = offsetYStart + (offsetYEnd * curveY.Evaluate(lnDelta));

            foreach (T0 loItem in goElements)
            {
                loPosition = loItem.transform.position;
                loPosition.y = lnY;
                loItem.transform.position = loPosition;
                loItem.transform.localScale = scaleStart + (scaleEnd * lnDeltaScale);
                loItem.SetAlpha(lnDeltaAlpha);
            }

            yield return loWaitForEndOfFrame;
            goMicroAnimation.Update();
        }
        while (goMicroAnimation.IsAnimating());

        yield break;
    }

    /// <summary>
    /// Establece un valor. En este caso de tiempo de esparcimiento.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        if (!float.TryParse(loValue as string, out float lnSpreadTime)) return;
        gnTime = lnSpreadTime;
    }

    /// <summary>
    /// Indica que debemos terminar con el prefab.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void Stop()
    {
        foreach (T0 loItem in goElements) loItem.gameObject.SetActive(false);
        goElements.Clear();
        gameObject.SetActive(false);
    }
}
