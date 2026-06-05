using System;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Instancia de GameManager por si algo la necesita.
    /// </summary>
    private static GameManager Instance = null;

    /// <summary>
    /// Url del sitio.
    /// </summary>
    private const string WebsiteURL = "https://localhost:7037/";

    /// <summary>
    /// Uri del sitio web.
    /// </summary>
    private static readonly Uri UriWebsite = new Uri(WebsiteURL);

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
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        StartLanguage();
        StartAdressables();
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
    }

    /// <summary>
    /// OnDestroy.
    /// </summary>
    private void OnDestroy()
    {
        OnDestroyCommunication();
    }
}