using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Instancia de GameManager por si algo la necesita.
    /// </summary>
    private static GameManager Instance = null;

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