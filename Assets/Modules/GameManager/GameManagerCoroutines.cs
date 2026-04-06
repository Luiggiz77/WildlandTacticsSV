using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private class RunObject
    {
        public string Id;
        public Action<bool, object> Callback;
        public object CallbackParameter;
        public Coroutine Coroutine;
        public MonoBehaviour Instance;
        public IEnumerator Function;
    };

    /// <summary>
    /// Nuestro diccionario de elementos creados.
    /// </summary>
    private static readonly Dictionary<string, RunObject> goRuns = new Dictionary<string, RunObject>();

    /// <summary>
    /// Nuestra lista de elementos creados.
    /// </summary>
    private static readonly List<RunObject> goQueuedRuns = new List<RunObject>();

    /// <summary>
    /// Es la coroutina que corre los que van en fila.
    /// </summary>
    public static Coroutine goQueueCoroutine = null;

    /// <summary>
    /// Se llama para crear coroutines y nos da el indice de la coroutine por si deseamos detenerla en algun punto.
    /// </summary>
    public static void RunQueued(IEnumerator loFunction, string lcId, Action<bool, object> loCallback = null, object loCallbackParameter = null)
    {
        RunQueued(Instance, loFunction, lcId, loCallback, loCallbackParameter);
    }

    /// <summary>
    /// Se llama para crear coroutines y nos da el indice de la coroutine por si deseamos detenerla en algun punto.
    /// </summary>
    public static void RunQueued<T>(T loInstance, IEnumerator loFunction, string lcId, Action<bool, object> loCallback = null, object loCallbackParameter = null) where T : MonoBehaviour
    {
        RunObject loRunObject = null;

        //<< Revisamos si existe un runobject con el mismo id para reemplazarlo, solo si no está siendo corrido.
        int lnCount = goQueuedRuns.Count;
        for (int i = 1; i < lnCount; i++)
        {
            if (goQueuedRuns[i].Id != lcId) continue;
            loRunObject = goQueuedRuns[i];
            break;
        }

        //<< Si no se encontro se crea y agrega.
        if (loRunObject == null)
        {
            loRunObject = new RunObject();
            goQueuedRuns.Add(loRunObject);
        }

        //<< Asignamos los datos de la coroutina.
        loRunObject.Id = lcId;
        loRunObject.Callback = loCallback;
        loRunObject.CallbackParameter = loCallbackParameter;
        loRunObject.Instance = loInstance;
        loRunObject.Function = loFunction;

        //<< Si ya está corriendo regresamos.
        if (goQueueCoroutine != null) return;

        goQueueCoroutine = loInstance.StartCoroutine(CoroutineQueued());
    }

    /// <summary>
    /// Corrutina que corre lo dado.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator CoroutineQueued()
    {
        RunObject loRunObject;
        while (goQueuedRuns.Count > 0)
        {
            loRunObject = goQueuedRuns[0];

            //<< Corremos la corrutina hasta que termine.
            yield return loRunObject.Function;
            loRunObject.Callback?.Invoke(true, loRunObject.CallbackParameter);

            goQueuedRuns.RemoveAt(0);
        }

        //<< Indicamos que terminamos de correr la fila.
        goQueueCoroutine = null;
        yield break;
    }

    /// <summary>
    /// Se llama para crear coroutines y nos da el indice de la coroutine por si deseamos detenerla en algun punto.
    /// </summary>
    public static void Run(IEnumerator loFunction, string lcId = "", Action<bool, object> loCallback = null, object loCallbackParameter = null)
    {
        Run(Instance, loFunction, lcId, loCallback, loCallbackParameter);
    }

    /// <summary>
    /// Se llama para crear coroutines y nos da el indice de la coroutine por si deseamos detenerla en algun punto.
    /// </summary>
    public static void Run<T>(T loInstance, IEnumerator loFunction, string lcId = "", Action<bool, object> loCallback = null, object loCallbackParameter = null) where T : MonoBehaviour
    {
        RunObject loRunObject;

        bool lbIsEmpty = string.IsNullOrWhiteSpace(lcId);

        //<< Primero buscamos de la lista si se han desactivado.
        if (!lbIsEmpty && goRuns.ContainsKey(lcId))
        {
            StopRun(lcId);
            loRunObject = goRuns[lcId];
        }
        else
        {
            loRunObject = new RunObject();
            if (!lbIsEmpty) goRuns.Add(lcId, loRunObject);
        }

        //<< Asignamos los datos de la coroutina.
        loRunObject.Callback = loCallback;
        loRunObject.CallbackParameter = loCallbackParameter;
        loRunObject.Instance = loInstance;
        loRunObject.Function = loFunction;
        loRunObject.Coroutine = loInstance.StartCoroutine(WrapperCoroutine(loRunObject));
    }

    /// <summary>
    /// Se llama para animar una funcion
    /// </summary>
    /// <param name="lcId"></param>
    /// <param name="loCallback"></param>
    /// <param name="loCallbackParameter"></param>
    public static void RunTimer(string lcId, float lnTime, Action<string> loOnUpdate, Action<bool, object> loCallback = null, object loCallbackParameter = null)
    {
        RunObject loRunObject;

        //<< Primero buscamos de la lista si se han desactivado.
        if (goRuns.ContainsKey(lcId))
        {
            StopRun(lcId);
            loRunObject = goRuns[lcId];
        }
        else
        {
            loRunObject = new RunObject();
            goRuns.Add(lcId, loRunObject);
        }

        //<< Asignamos los datos de la coroutina.
        loRunObject.Callback = loCallback;
        loRunObject.CallbackParameter = loCallbackParameter;
        loRunObject.Instance = Instance;
        loRunObject.Coroutine = Instance.StartCoroutine(Timer(loRunObject, lnTime, loOnUpdate));
    }

    /// <summary>
    /// Corremos un timer con base en los segundos.
    /// </summary>
    /// <param name="lnTime"></param>
    /// <returns></returns>
    private static IEnumerator Timer(RunObject loRunObject, float lnTime, Action<string> loOnUpdate = null)
    {
        float lnEndTime = Time.time + lnTime;
        int lnLastReportedSeconds = -1;
        float lnRemainingSeconds;
        int lnRemainingSecondsCeil;

        while (Time.time < lnEndTime)
        {
            lnRemainingSeconds = lnEndTime - Time.time;
            lnRemainingSecondsCeil = Mathf.CeilToInt(lnRemainingSeconds);

            //<< Solo actualiza el texto si el valor cambió
            if (lnRemainingSecondsCeil != lnLastReportedSeconds)
            {
                System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(lnRemainingSecondsCeil);
                loOnUpdate?.Invoke($"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");
                lnLastReportedSeconds = lnRemainingSecondsCeil;
            }

            yield return null; //<< Espera un frame, pero no siempre actualiza texto
        }

        //<< Asegura mostrar 00:00 al final
        loOnUpdate?.Invoke("00:00");

        loRunObject.Callback?.Invoke(true, loRunObject.CallbackParameter);
        loRunObject.Coroutine = null;
    }

    /// <summary>
    /// Corrutina que corre lo dado.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator WrapperCoroutine(RunObject loRunObject)
    {
        //<< Corremos la corrutina hasta que termine.
        yield return loRunObject.Function;
        loRunObject.Callback?.Invoke(true, loRunObject.CallbackParameter);
        loRunObject.Coroutine = null;
        yield break;
    }

    /// <summary>
    /// Detiene todos los elementos.
    /// </summary>
    public static void StopRuns()
    {
        foreach (KeyValuePair<string, RunObject> loRunObject in goRuns)
        {
            if (loRunObject.Value.Coroutine == null) continue;
            Instance.StopCoroutine(loRunObject.Value.Coroutine);
            loRunObject.Value.Callback?.Invoke(false, loRunObject.Value.CallbackParameter);
            loRunObject.Value.Coroutine = null;
        }
    }

    /// <summary>
    /// Para desactivar los elementos especificos.
    /// </summary>
    /// <param name="lcId"></param>
    public static void StopRun(string lcId)
    {
        if (!goRuns.ContainsKey(lcId)) return;
        RunObject loRunObject = goRuns[lcId];
        if (loRunObject.Coroutine == null) return;
        if (loRunObject.Instance != null) loRunObject.Instance.StopCoroutine(loRunObject.Coroutine);
        loRunObject.Coroutine = null;
        loRunObject.Callback?.Invoke(false, loRunObject.CallbackParameter);
    }
}