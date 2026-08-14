using System.Collections;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Instancia de GameManager por si algo la necesita.
    /// </summary>
    private static GameManager Instance = null;

    /// <summary>
    /// Nuestra lista de elementos creados.
    /// </summary>
    private readonly System.Collections.Generic.List<IEnumerator> goStartCoroutines = new System.Collections.Generic.List<IEnumerator>();

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        Application.targetFrameRate = 30;
        Instance = this;

        AwakeAudio();
        AwakeTexture2D();
        AwakeWebsite();
        AwakeLanguage();
        AwakeSignalR();
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        StartLanguage();
        StartWebsite();
        StartAdressables();

        //<< Iniciamos las corrutinas que van en el start.
        StartCoroutine(StartCoroutines());
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        UpdateSignalR();
    }

    /// <summary>
    /// OnDestroy.
    /// </summary>
    private void OnDestroy()
    {
        OnDestroyCommunication();
        OnDestroyAddressables();
    }

    /// <summary>
    /// Iniciamos coroutines iniciales.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCoroutines()
    {
        //<< Avisamos a la pantalla de carga que debe mostrarse.
        GameManager.Send(GameCommand.ShowUIDownloading);

        foreach (IEnumerator loCoroutine in goStartCoroutines) yield return loCoroutine;

        goStartCoroutines.Clear();

        //<< Avisamos a la pantalla de carga que debe ocultarse.
        GameManager.Send(GameCommand.HideUIDownloading);

        //<< Avisamos que debemos ir al menu principal.
        GameManager.Send(GameCommand.ShowUIMainMenu);

        yield break;
    }
}