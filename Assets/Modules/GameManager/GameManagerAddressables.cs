using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Handle del catalogo.
    /// </summary>
    private AsyncOperationHandle<IResourceLocator> goCatalogHandle;

    /// <summary>
    /// Handle de las GameTexture2D.
    /// </summary>
    private AsyncOperationHandle<IList<Texture2D>> goGameTextures2DHandle;

    /// <summary>
    /// Se llama ane le start del GameManager
    /// </summary>
    private void StartAdressables()
    {
        StartCoroutine(InitializeAddressables());
    }

    /// <summary>
    /// Nos indica si debemos o no reiniciar la inicialización.
    /// </summary>
    /// <param name="lbRetry"></param>
    private void RestartInitializeAdressables(bool lbRetry)
    {
        if (lbRetry) StartAdressables();
        else Application.Quit();
    }

    /// <summary>
    /// Nos indica si debemos o no reiniciar la descarga del catalogo de los addressables.
    /// </summary>
    /// <param name="lbRetry"></param>
    private void RestartDownloadCatalog(bool lbRetry)
    {
        if (lbRetry) StartCoroutine(DownloadCatalog());
        else Application.Quit();
    }

    /// <summary>
    /// Nos indica si debemos o no reiniciar la descarga de los Addressables.
    /// </summary>
    /// <param name="lbRetry"></param>
    private void RestartDownloadAddressables(bool lbRetry)
    {
        if (lbRetry) StartCoroutine(DownloadAddressables());
        else Application.Quit();
    }

    /// <summary>
    /// Nos indica si debemos o no reiniciar la carga de las gameTextures2D.
    /// </summary>
    /// <param name="lbRetry"></param>
    private void RestartLoadGameTextures2D(bool lbRetry)
    {
        if (lbRetry) StartCoroutine(LoadGameTextures2D());
        else Application.Quit();
    }

    /// <summary>
    /// Es para inicializar los Addressables.
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeAddressables()
    {
        //<< Avisamos a la pantalla de descarga que debe mostrarse.
        GameManager.Send(GameCommand.ShowUIDownloading);

        //<< Inicializamos Addressables
        AsyncOperationHandle loInitHandle = Addressables.InitializeAsync(false);

        //<< Esperamos se inicialicen los addressables.
        yield return loInitHandle;

        //<< Revisamos que Addressables se inicializó correctamente.
        if (loInitHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loInitHandle);
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartInitializeAdressables));
            yield break;
        }

        //<< Podemos liberar el handle ya que obtuvimos el resultado.
        Addressables.Release(loInitHandle);

        //<< Ya que inicializamos los Addressables pasamos a obtener el catalogo.
        yield return DownloadCatalog();
        yield break;
    }

    /// <summary>
    /// Se llama para descargar el catalogo.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadCatalog()
    {
        //<< Avisamos a la pantalla de carga que debe mostrarse.
        GameManager.Send(GameCommand.ShowUIDownloading);

        //<< Debemos descargar primero el archivo de la versión para saber el apendice de los archivos ".bin" y ".hash".
        CoroutineResult<string> loDownloadVersionFileResult = new CoroutineResult<string>();
        yield return DownloadTextFile(UriVersionFile, loDownloadVersionFileResult);

        if (!loDownloadVersionFileResult.Completed)
        {
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartDownloadCatalog));
            Debug.Log($"Error al descargar el archivo de version. Error: {loDownloadVersionFileResult.Object}");
            yield break;
        }

        //<< Generamos la direccion del catalogo de addressables.
        string lcCatalogURL = $"{WebsiteURL}{GetPlatformPath()}/catalog_{loDownloadVersionFileResult.Object}.bin";

        //<< Indicamos que no se busque en StreamingAssets si no que use la URL como base.
        AddressablesRuntimeProperties.SetPropertyValue("RemoteCatalogProvider", lcCatalogURL);

        //<< Cargamos los datos del catalogo de Addressables.
        goCatalogHandle = Addressables.LoadContentCatalogAsync(lcCatalogURL, false);
        yield return goCatalogHandle;

        //<< Revisamos si se logró obtener el catalogo.
        if (goCatalogHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(goCatalogHandle);
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartDownloadCatalog));
            yield break;
        }

        //<< Intentamos la descarga de los elementos.
        yield return DownloadAddressables();
        yield break;
    }

    /// <summary>
    /// Corrutina de descarga de todos los Addressables.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadAddressables()
    {
        //<< Avisamos a la pantalla de carga que debe mostrarse.
        GameManager.Send(GameCommand.ShowUIDownloading);

        //<< Iniciamos la descarga.
        AsyncOperationHandle loDownloadDependenciesHandle = Addressables.DownloadDependenciesAsync("default", false);

        //<< Actualizamos el porcentaje en la UIDownloading.
        while (!loDownloadDependenciesHandle.IsDone)
        {
            GameManager.Send(GameCommand.SetUIDownloadingPercent, loDownloadDependenciesHandle.PercentComplete);
            yield return null;
        }

        if (loDownloadDependenciesHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(loDownloadDependenciesHandle);
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartDownloadAddressables));
            yield break;
        }

        Addressables.Release(loDownloadDependenciesHandle);

        //<< Cargamos las texturas 2D.
        yield return LoadGameTextures2D();
        yield break;
    }

    /// <summary>
    /// Carga de Textures2D.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadGameTextures2D()
    {
        //<< Obtenemos las locaciones de las texturas.
        string[] loKeys = new string[] { nameof(GameTexture2D) };
        AsyncOperationHandle<IList<IResourceLocation>> loLocationsHandle = Addressables.LoadResourceLocationsAsync(loKeys, Addressables.MergeMode.Intersection, typeof(Texture2D));
        yield return loLocationsHandle;

        if(loLocationsHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loLocationsHandle);
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartLoadGameTextures2D));
            yield break;
        }

        //<< Obtenemos referencia de las locaciones.
        IList<IResourceLocation> loLocations = loLocationsHandle.Result;
        
        //<< Obtenemos las texturas.
        goGameTextures2DHandle = Addressables.LoadAssetsAsync<Texture2D>(nameof(GameTexture2D), callback: null, Addressables.MergeMode.Union);

        yield return goGameTextures2DHandle;

        //<< Si no se logró reintentamos.
        if (goGameTextures2DHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(loLocationsHandle);
            Addressables.Release(goGameTextures2DHandle);
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, new UnityAction<bool>(RestartLoadGameTextures2D));
            yield break;
        }

        //<< Las locaciones y Textures2D estan "emparejadas".
        string[] loSplitString;
        string lcColor;
        int lnUsage, lnKey;
        GameTexture2DUsage loGameTexture2DUsage;
        Color loColor;
        int lnCount = loLocations.Count;
        for (int i = 0; i < lnCount; i++)
        {
            loSplitString = loLocations[i].PrimaryKey.Split("_");
            if (loSplitString.Length < 3) continue;
            if (!int.TryParse(loSplitString[0], out lnUsage)) continue;
            if (!int.TryParse(loSplitString[1], out lnKey)) continue;

            lcColor = loSplitString[2];
            if (!lcColor.StartsWith("#")) lcColor = "#" + lcColor;
            if (!ColorUtility.TryParseHtmlString(lcColor, out loColor)) loColor = Color.white;

            loGameTexture2DUsage = (GameTexture2DUsage)lnUsage;

            //<< Revisamos si ya tenemos la llave "principal".
            if (!goGameTextures2D.ContainsKey(loGameTexture2DUsage)) goGameTextures2D.Add(loGameTexture2DUsage, new List<GameTexture2D>());

            //<< Convertimos la llave a nuestros elementos de diccionario.
            goGameTextures2D[loGameTexture2DUsage].Add(new GameTexture2D(lnKey, loColor, goGameTextures2DHandle.Result[i]));
        }

        //<< Liberamos handle de las locaciones.
        Addressables.Release(loLocationsHandle);

        //<< Avisamos a la pantalla de carga que debe ocultarse.
        GameManager.Send(GameCommand.HideUIDownloading);

        //<< Avisamos que debemos ir al menu principal.
        GameManager.Send(GameCommand.ShowUIMainMenu);
    }

    /// <summary>
    /// Se llama en GameManager.
    /// </summary>
    private void OnDestroyAddressables()
    {
        if (goGameTextures2DHandle.IsValid()) Addressables.Release(goGameTextures2DHandle);
        if (goCatalogHandle.IsValid()) Addressables.Release(goCatalogHandle);
    }
}