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
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        StartWebsite();
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