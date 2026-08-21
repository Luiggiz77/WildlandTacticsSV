using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    /// <summary>
    /// Para activar o desactivar elementos con componente.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loElements"></param>
    /// <param name="lbActive"></param>
    public static void SetActive<T>(this IEnumerable<T> loElements, bool lbActive) where T : Component
    {
        foreach (T loItem in loElements) loItem.gameObject.SetActive(lbActive);
    }

    /// <summary>
    /// Se llama para establecer la posicion de los componentes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loElements"></param>
    /// <param name="loPosition"></param>
    public static void SetPosition<T>(this IEnumerable<T> loElements, Vector3 loPosition) where T : Component
    {
        foreach (T loItem in loElements) loItem.transform.position = loPosition;
    }
}