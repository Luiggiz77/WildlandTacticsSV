using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class UILoading : UIView
{
    /// <summary>
    /// Instancia del loading.
    /// </summary>
    public static UILoading goInstance = null;

    /// <summary>
    /// Corrutinas que debemos correr.
    /// </summary>
    private List<IEnumerator> goCoroutines = new List<IEnumerator>();

    /// <summary>
    /// Start
    /// </summary>
    public void Start()
    {
        goInstance = this;
    }

    /// <summary>
    /// Nos indica si está activo el loading.
    /// </summary>
    /// <returns></returns>
    public static bool Active()
    {
        if (goInstance == null) return false;
        return goInstance.panel.activeSelf;
    }

    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUILoading:
                transform.SetAsLastSibling();
                panel.SetActive(true);

                //<< Buscamos lugar para la coroutina.
                QueueCoroutine((IEnumerator)loParams[0]);
                break;
            default: break;
        }
    }

    /// <summary>
    /// Agrega la coroutina
    /// </summary>
    /// <param name="loCoroutine"></param>
    private void QueueCoroutine(IEnumerator loCoroutine)
    {
        int lnCount = goCoroutines.Count;
        for (int i = 0; i < lnCount; i++)
        {
            if (goCoroutines[i] != null) continue;
            goCoroutines[i] = loCoroutine;
            StartCoroutine(WrapperCoroutine(loCoroutine, i, OnCoroutineEnded));
            return;
        }

        //<< Si no hubo espacio le creamos uno para correr la corrutina.
        goCoroutines.Add(loCoroutine);
        StartCoroutine(WrapperCoroutine(loCoroutine, lnCount, OnCoroutineEnded));
    }

    /// <summary>
    /// Callback para indicarnos que la coroutina ha terminado.
    /// </summary>
    /// <param name="lnCoroutineIndex"></param>
    private void OnCoroutineEnded(int lnCoroutineIndex)
    {
        //<< Removemos la coroutina de la lista.
        goCoroutines[lnCoroutineIndex] = null;

        //<< Revisamos si existen coroutinas pendientes antes de cerrar el loading.
        int lnCount = goCoroutines.Count;
        for (int i = 0; i < lnCount; i++)
        {
            if (goCoroutines[i] != null) return;
        }

        //<< Ya que todas corrutinas han temrinado cerramos la pantalla de carga.
        panel.SetActive(false);
    }

    /// <summary>
    /// Corrutina que corre lo dado.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator WrapperCoroutine(IEnumerator loCoroutine, int lnCoroutineIndex, UnityAction<int> loCallback)
    {
        //<< Corremos la corrutina hasta que termine.
        yield return loCoroutine;

        //<< Avisamos la que corrutina ha terminado.
        loCallback.Invoke(lnCoroutineIndex);

        yield break;
    }
}