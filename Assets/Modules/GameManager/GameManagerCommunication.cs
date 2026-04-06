using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager : MonoBehaviour
{
#if UNITY_EDITOR
    /// <summary>
    /// Indica si debemos mostrar quien manda mensajes y ver que mandan.
    /// </summary>
    public static bool Verbose = false;
#endif

    /// <summary>
    /// Para poder mandar los comandos a todos los objetos registrados.
    /// Nota: Se limpia al cambiar de escena (en caso de cambiar de escena).
    /// </summary>
    private static readonly List<UnityAction<GameCommand, object[]>> goWire = new List<UnityAction<GameCommand, object[]>>();

    /// <summary>
    /// Se llama para registrarnos al evento de comunicación.
    /// </summary>
    /// <param name="loAction"></param>
    public static void AddListener(UnityAction<GameCommand, object[]> loAction)
    {
        goWire.Remove(loAction);
        goWire.Add(loAction);
    }

    /// <summary>
    /// Se llama para removernos del evento de comunicación.
    /// </summary>
    /// <param name="loAction"></param>
    public static void RemoveListener(UnityAction<GameCommand, object[]> loAction)
    {
        goWire.Remove(loAction);
    }

    /// <summary>
    /// Se usa para comunicar algo a los objetos registrados.
    /// </summary>
    /// <param name="lcFrom">Debe ser de las constantes del objeto estatico GameEntitys</param>
    /// <param name="loCommand">Debe ser de las constantes del objeto estatico GameCommands</param>
    /// <param name="loParams"></param>
    public static void Send(GameCommand loCommand, params object[] loParams)
    {
#if UNITY_EDITOR
        //<< Si nos indican que mostremos que mandan...
        if (Verbose)
        {
            Debug.Log($"Send - Command: {loCommand}");
            int lnCount = loParams.Length;
            for (int i = 0; i < lnCount; i++) Debug.Log(loParams[i].ToString());
        }
#endif

        //<< Mandamos mensaje a todos y el que ya no exista se remueve.
        int lnCountActions = goWire.Count;
        for (int i = lnCountActions - 1; i >= 0; i--)
        {
            if (goWire[i] == null)
            {
                goWire.RemoveAt(i);
                continue;
            }
            else goWire[i].Invoke(loCommand, loParams);
        }
    }

    /// <summary>
    /// Se llama cuando se destruye el componente.
    /// </summary>
    private void OnDestroyCommunication()
    {
        goWire.Clear();
    }
}