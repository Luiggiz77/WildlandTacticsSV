using System.Collections.Generic;
using UnityEngine;

public abstract class PlayablePrefab : MonoBehaviour
{
    /// <summary>
    /// Nos indican que debemos terminar.
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Nos indica si ya terminó el playable prefab.
    /// </summary>
    /// <returns></returns>
    public abstract bool Ended();

    /// <summary>
    /// Nos pasaran un valor que puede significar algo.
    /// </summary>
    public abstract void SetValue(object loValue);

    /// <summary>
    /// Nos indica que debemos iniciar.
    /// </summary>
    public abstract void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells);

    /// <summary>
    /// Nos indica que debemos mostrar un efecto sin aplicar el play, como por ejemplo mostrar un proyectil de fuego pero sin aplicar su translación.
    /// </summary>
    public abstract void Preview();

    /// <summary>
    /// Nos indica que debemos reproducir de manera asincrona.
    /// </summary>
    public abstract System.Collections.IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells);

    /// <summary>
    /// Nos indica que debemos reproducir de manera asincrona con las dependencias de posiciones y deltas efectuados.
    /// (Se usa solo para los efectos de area que se dividen en secciones)
    /// </summary>
    public abstract System.Collections.IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells);
}