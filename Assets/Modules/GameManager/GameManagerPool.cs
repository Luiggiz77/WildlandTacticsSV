using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Nuestro diccionario de elementos creados.
    /// </summary>
    private static readonly Dictionary<int, List<Component>> goSpawns = new Dictionary<int, List<Component>>();

    /// <summary>
    /// Establecemos la posicion y rotacion de la particula y reproducimos.
    /// </summary>
    /// <param name="loType"></param>
    /// <param name="loPosition"></param>
    public static T Spawn<T>(int lnHashCode, T loComponent) where T : Component
    {
        //<< Si no hay llave del elemento lo agregamos.
        if (!goSpawns.ContainsKey(lnHashCode)) goSpawns.Add(lnHashCode, new List<Component>());

        //<< Primero buscamos de las particulas que se han desactivado.
        List<Component> loSpawns = goSpawns[lnHashCode];
        foreach (Component loItem in loSpawns)
        {
            if (loItem.gameObject.activeSelf) continue;
            loItem.gameObject.SetActive(true);
            return (T)loItem;
        }

        //<< Ya que no hay ningun elemento desocupado creamos uno mas.
        Component loInstantiated = Instantiate(loComponent);
        loInstantiated.transform.localScale = Vector3.one;
        loInstantiated.gameObject.SetActive(true);
        loSpawns.Add(loInstantiated);
        return (T)loInstantiated;
    }

    /// <summary>
    /// Desactiva todos los elementos.
    /// </summary>
    /// <param name="loType"></param>
    public static void DeactivateSpawns()
    {
        foreach (KeyValuePair<int, List<Component>> loKeyValuePair in goSpawns)
        {
            foreach (Component loComponent in loKeyValuePair.Value) loComponent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Para desactivar los elementos especificos.
    /// </summary>
    /// <param name="lnHashCode"></param>
    public static void DeactivateSpawns(int lnHashCode)
    {
        if (!goSpawns.ContainsKey(lnHashCode)) return;
        List<Component> loComponents = goSpawns[lnHashCode];
        foreach (Component loComponent in loComponents) loComponent.gameObject.SetActive(false);
    }


    /// <summary>
    /// Nos da un elemento intanciado basandonos en el id de la instancia pero sobre el componente.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lnHashCode"></param>
    /// <param name="lnInstanceId"></param>
    /// <returns></returns>
    public static T GetSpawnWithInstanceId<T>(int lnHashCode, int lnInstanceId) where T : Component
    {
        if (!goSpawns.ContainsKey(lnHashCode)) return null;
        List<Component> loComponents = goSpawns[lnHashCode];
        foreach (Component loItem in loComponents)
        {
            if (loItem.GetInstanceID() == lnInstanceId) return loItem as T;
        }
        return null;
    }

    /// <summary>
    /// Nos da un elemento intanciado basandonos en el id de la instancia pero sobre el gameobject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lnHashCode"></param>
    /// <param name="lnInstanceId"></param>
    /// <returns></returns>
    public static T GetSpawnWithGameObjectInstanceId<T>(int lnHashCode, int lnInstanceId) where T : Component
    {
        if (!goSpawns.ContainsKey(lnHashCode)) return null;
        List<Component> loComponents = goSpawns[lnHashCode];
        foreach (Component loItem in loComponents)
        {
            if (loItem.gameObject.GetInstanceID() == lnInstanceId) return loItem as T;
        }
        return null;
    }

    /// <summary>
    /// Nos da un elemento intanciado basandonos en el nombre.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lnHashCode"></param>
    /// <param name="lcName"></param>
    /// <returns></returns>
    public static T GetSpawnWithName<T>(int lnHashCode, string lcName) where T : Component
    {
        if (!goSpawns.ContainsKey(lnHashCode)) return null;
        List<Component> loComponents = goSpawns[lnHashCode];
        foreach (Component loItem in loComponents)
        {
            if (loItem.name == lcName) return loItem as T;
        }
        return null;
    }

    /// <summary>
    /// Nos da un conjunto de elementos intanciados basandonos en el inicio del nombre.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lnHashCode"></param>
    /// <param name="lcName"></param>
    /// <param name="loResults">Indica el total de resultados para evitar GC.</param>
    public static void GetSpawnsStartsWithName<T>(int lnHashCode, string lcName, T[] loResults) where T : Component
    {
        if (!goSpawns.ContainsKey(lnHashCode)) return;
        int lnResults = loResults.Length;
        if (lnResults == 0) return;
        int lnIndex = 0;
        List<Component> loComponents = goSpawns[lnHashCode];
        foreach (Component loItem in loComponents)
        {
            if (!loItem.name.StartsWith(lcName)) continue;
            loResults[lnIndex] = loItem as T;
            lnIndex++;
            if (lnIndex == lnResults) return;
        }
    }
}
