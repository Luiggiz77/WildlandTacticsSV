using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class GameManager : MonoBehaviour
{
    // Puedes usar la etiqueta "Preload" en Unity para marcar todo lo que se debe bajar al inicio
    public string PreDownloadLabel = "Preload";

    /// <summary>
    /// Se llama ane le start del GameManager
    /// </summary>
    void StartAdressables()
    {
        StartCoroutine(DownloadAllRoutine());
    }

    /// <summary>
    /// Corrutina de descarga de todo lo descargable.
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadAllRoutine()
    {
        //<< Avisamos a la pantalla de carga que debe mostrarse.
        GameManager.Send(GameCommand.ShowUIDownloading);

        //<< Inicializamos Addressables
        AsyncOperationHandle loInitHandle = Addressables.InitializeAsync();

        //<< Esperamos se inicialicen los adressables.
        yield return loInitHandle;

        // 2. LE DECIMOS DÓNDE ESTÁ EL CATÁLOGO REMOTO
        // (Apunta directamente al archivo .json que subió el proyecto productor a Blazor)
        string lcCatalogURL = "https://localhost:7037/catalog.bin";

        AsyncOperationHandle<IResourceLocator> loCatalogHandle = Addressables.LoadContentCatalogAsync(lcCatalogURL, true);
        yield return loCatalogHandle;

        if (loCatalogHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("¡Catálogo remoto cargado con éxito! Ahora el cliente ya conoce la etiqueta 'default'.");

            //<< Comprobamos el tamaño de la descarga.
            AsyncOperationHandle<long> loDownloadSizeHandle = Addressables.GetDownloadSizeAsync("default");
            yield return loDownloadSizeHandle;

            //<< Si todo va bien...
            if (loDownloadSizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                long lnDownloadSize = loDownloadSizeHandle.Result;

                //<< Si el tamaño es mayor a 0 hay cosas por descargar.
                if (lnDownloadSize > 0)
                {
                    Debug.Log($"Hay contenido nuevo para descargar: {lnDownloadSize / (1024f * 1024f):F2} MB");

                    //<< Iniciamos la descarga.
                    AsyncOperationHandle loDownloadDependenciesHandle = Addressables.DownloadDependenciesAsync(PreDownloadLabel, true);

                    //<< Actualizamos el porcentaje en la UIDownloading.
                    while (!loDownloadDependenciesHandle.IsDone)
                    {
                        GameManager.Send(GameCommand.SetUIDownloadingPercent, loDownloadDependenciesHandle.PercentComplete);
                        yield return null;
                    }

                    if (loDownloadDependenciesHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("¡Descarga completada con éxito!");
                    }
                    else
                    {
                        Debug.LogError("Error al descargar los Addressables.");
                    }

                    Addressables.Release(loDownloadDependenciesHandle);
                }
                else
                {
                    Debug.Log("Todo está al día. No hay nada que descargar.");
                }
            }
            //<< Si ocurre un error mandamos modal de reintentar.
            else
            {
                Debug.LogError("Error al verificar el tamaño de la descarga.");
            }
        }
        else
        {
            Debug.LogError("No se pudo descargar el catálogo del servidor Blazor. ¿Está encendido el servidor o bien configurados los MIME types?");
        }

        //<< Avisamos a la pantalla de carga que debe ocultarse.
        GameManager.Send(GameCommand.HideUIDownloading);
    }
}