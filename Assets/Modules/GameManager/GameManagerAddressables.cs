using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Se llama ane le start del GameManager
    /// </summary>
    void StartAdressables()
    {
        StartCoroutine(DownloadAllRoutine());
    }

    /// <summary>
    /// Nos indica si debemos o no reiniciar la descarga de adressables.
    /// </summary>
    /// <param name="lbRetry"></param>
    private void RestartAdressables(bool lbRetry)
    {
        if (lbRetry) StartAdressables();
        else Application.Quit();
    }

    /// <summary>
    /// Corrutina de descarga de todo lo descargable.
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadAllRoutine()
    {
        //<< Avisamos a la pantalla de carga que debe mostrarse.
        GameManager.Send(GameCommand.ShowUIDownloading);

        //<< Debemos descargar primero el archivo de la versión para saber el apendice de los archivos ".bin" y ".hash".
        CoroutineResult<string> loDownloadVersionFileResult = new CoroutineResult<string>();
        yield return DownloadTextFile(UriVersionFile, loDownloadVersionFileResult);

        if (!loDownloadVersionFileResult.Completed)
        {
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartAdressables));
            Debug.Log($"Error al descargar el archivo de version. Error: {loDownloadVersionFileResult.Object}");
            yield break;
        }

        //<< Generamos la direccion del catalogo de addressables.
        string lcCatalogURL = $"{WebsiteURL}{GetPlatformPath()}/catalog_{loDownloadVersionFileResult.Object}.bin";

        // 3. ¡LA MAGIA! Creamos un objeto de inicialización manual
        // Esto le dice a Unity: "No busques en StreamingAssets, usa esta URL como base"
        AddressablesRuntimeProperties.SetPropertyValue("RemoteCatalogProvider", lcCatalogURL);

        //<< Inicializamos Addressables
        AsyncOperationHandle loInitHandle = Addressables.InitializeAsync(false);

        //<< Esperamos se inicialicen los adressables.
        yield return loInitHandle;

        //<< Revisamos que Addressables se inicializó correctamente.
        if (loInitHandle.Status == AsyncOperationStatus.Failed)
        {
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartAdressables));
            Addressables.Release(loInitHandle);
            yield break;
        }

        Addressables.Release(loInitHandle);

        AsyncOperationHandle<IResourceLocator> loCatalogHandle = Addressables.LoadContentCatalogAsync(lcCatalogURL, false);
        yield return loCatalogHandle;

        if (loCatalogHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(loCatalogHandle);
            
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
                    //<< Iniciamos la descarga.
                    AsyncOperationHandle loDownloadDependenciesHandle = Addressables.DownloadDependenciesAsync("default", false);

                    //<< Actualizamos el porcentaje en la UIDownloading.
                    while (!loDownloadDependenciesHandle.IsDone)
                    {
                        GameManager.Send(GameCommand.SetUIDownloadingPercent, loDownloadDependenciesHandle.PercentComplete);
                        yield return null;
                    }

                    Addressables.Release(loDownloadDependenciesHandle);

                    if (loDownloadDependenciesHandle.Status != AsyncOperationStatus.Succeeded)
                    {
                        GameManager.Send(GameCommand.HideUIDownloading);
                        GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartAdressables));
                        yield break;
                    }
                }
            }
            //<< Si ocurre un error mandamos modal de reintentar.
            else
            {
                GameManager.Send(GameCommand.HideUIDownloading);
                GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartAdressables));
                yield break;
            }
        }
        else
        {
            Addressables.Release(loCatalogHandle);
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartAdressables));
            yield break;
        }

        //<< Avisamos a la pantalla de carga que debe ocultarse.
        GameManager.Send(GameCommand.HideUIDownloading);
    }
}