using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public partial class GameManager : MonoBehaviour
{
    private static class AddressableLabel
    {
        /// <summary>
        /// Llave para descargar los addressables.
        /// </summary>
        public const string Default = "default";

        /// <summary>
        /// Llave para descargar los shared shaders.
        /// </summary>
        public const string SharedShaders = "sharedshaders";
    }

    /// <summary>
    /// Handle del catalogo.
    /// </summary>
    private AsyncOperationHandle<IResourceLocator> goCatalogHandle;

    /// <summary>
    /// Handle de las GameTexture2D.
    /// </summary>
    private AsyncOperationHandle<IList<Texture2D>> goGameTextures2DHandle;

    /// <summary>
    /// Handle de los unit instances.
    /// </summary>
    private AsyncOperationHandle<IList<GameObject>> goUnitInstancesHandle;

    /// <summary>
    /// Handle de los unit elements.
    /// </summary>
    private AsyncOperationHandle<IList<GameObject>> goUnitElementsHandle;

    /// <summary>
    /// Lista de unit instances que son partes que componen a las unidades, como cabeza, cuerpo, props, etc...
    /// </summary>
    private Dictionary<int, UnitAssetBundle> goUnitAssetBundles = new Dictionary<int, UnitAssetBundle>();

    /// <summary>
    /// Se llama ane le start del GameManager
    /// </summary>
    private void StartAdressables()
    {
        goStartCoroutines.Add(Setup());
    }

    /// <summary>
    /// Carga todo lo necesario dentro de una sola corrutina.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Setup()
    {
        CoroutineResult loCoroutineResult = new CoroutineResult();
        CoroutineResultValue<long> loCoroutineResultSize = new CoroutineResultValue<long>();

        //<< Inicializamos los addressables.
        yield return RunManagedWhile(InitializeAddressables(loCoroutineResult), loCoroutineResult);

        //<< Obtenemos el catalogo.
        yield return RunManagedWhile(DownloadCatalog(loCoroutineResult), loCoroutineResult);

        //foreach (var locator in UnityEngine.AddressableAssets.Addressables.ResourceLocators)
        //{
        //    foreach (var key in locator.Keys) Debug.Log($"Llave: '{key}'");
        //}

        //<< Obtenemos el tamaño de descarga de todos los elementos default.
        yield return RunManagedWhile(GetDownloadSize(AddressableLabel.Default, loCoroutineResult, loCoroutineResultSize), loCoroutineResult);

        //<< FINDME Aqui debemos indicarle el tamaño a la pantalla de Descarga. (Default)
        Debug.Log($"Tamaño de descarga elementos default: {loCoroutineResultSize.Value} KB");

        //<< Obtenemos elementos default.
        yield return RunManagedWhile(DownloadAddressables(AddressableLabel.Default, loCoroutineResult), loCoroutineResult);

        //<< Cargamos los shader.

        //<< Cargamos las Texture2D en memoria.
        yield return RunManagedWhile(LoadGameTextures2D(loCoroutineResult), loCoroutineResult);

        //<< Cargamos los UnitAssetBundles (UnitInstances) en memoria.
        yield return RunManagedWhile(LoadUnitAssetBundles_UnitInstance(loCoroutineResult), loCoroutineResult);

        //<< Cargamos los UnitAssetBundles (UnitElements) en memoria.
        yield return RunManagedWhile(LoadUnitAssetBundles_UnitElement(loCoroutineResult), loCoroutineResult);
    }

    /// <summary>
    /// Corre un loop while controlado por un modal en caso de errores.
    /// </summary>
    /// <param name="loCoroutine"></param>
    /// <param name="loCoroutineResult"></param>
    /// <returns></returns>
    private static IEnumerator RunManagedWhile(IEnumerator loCoroutine, CoroutineResult loCoroutineResult)
    {
        while (true)
        {
            //<< Avisamos a la pantalla de descarga que debe mostrarse.
            GameManager.Send(GameCommand.ShowUIDownloading);

            //<< Procesamos la coroutine que nos indican.
            yield return loCoroutine;

            //<< Si se logra correctamente salimos.
            if (loCoroutineResult.Succeeded) yield break;

            //<< Si hubo error lo indicamos en el modal.
            GameManager.Send(GameCommand.HideUIDownloading);
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.Retry), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Retry), null, null, loCoroutineResult);

            //<< Esperamos respuesta en el modal atraves del objeto de resultado.
            while (!loCoroutineResult.Completed) yield return null;

            //<< Si nos indican que reintentemos...
            if (loCoroutineResult.Succeeded) continue;

            //<< Ya que nos indican que no desean reintentar entonces salimos de la app por que los datos de esta sección son indispensables para continuar.
            Application.Quit();
            yield break;
        }
    }

    /// <summary>
    /// Obtenemos el tamaño de descarga de todo lo descargable.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator GetDownloadSize(string lcKey, CoroutineResult loCoroutineResult, CoroutineResultValue<long> loResult)
    {
        //<< Obtenemos el tamaño de descarga.
        AsyncOperationHandle<long> loDownloadSizeHandle = Addressables.GetDownloadSizeAsync(lcKey);
        yield return loDownloadSizeHandle;

        //<< Revisamos si se logró obtener el tamaño de descarga.
        if (loDownloadSizeHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loDownloadSizeHandle);
            loCoroutineResult.Failed();
            loResult.Failed(0);
            yield break;
        }

        //<< Obtenemos el tamaño de descarga.
        loResult.Succeed(loDownloadSizeHandle.Result);

        //<< Liberamos el tamaño porque ya tenemos el tamaño.
        Addressables.Release(loDownloadSizeHandle);
        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Corrutina generica para descargar Addressables.
    /// </summary>
    /// <param name="lcKey"></param>
    /// <param name="loOnFail"></param>
    /// <param name="loNext"></param>
    /// <returns></returns>
    private IEnumerator DownloadAddressables(string lcKey, CoroutineResult loCoroutineResult)
    {
        //<< Avisamos a la pantalla de carga que debe mostrarse.
        GameManager.Send(GameCommand.SetUIDownloadingPercent, 0.0f);

        //<< Iniciamos la descarga.
        AsyncOperationHandle loDownloadDependenciesHandle = Addressables.DownloadDependenciesAsync(lcKey, false);

        //<< Actualizamos el porcentaje en la UIDownloading.
        while (!loDownloadDependenciesHandle.IsDone)
        {
            GameManager.Send(GameCommand.SetUIDownloadingPercent, loDownloadDependenciesHandle.PercentComplete);
            yield return null;
        }

        if (loDownloadDependenciesHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(loDownloadDependenciesHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        Addressables.Release(loDownloadDependenciesHandle);

        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Es para inicializar los Addressables.
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeAddressables(CoroutineResult loCoroutineResult)
    {
        //<< Inicializamos Addressables
        AsyncOperationHandle loInitHandle = Addressables.InitializeAsync(false);

        //<< Esperamos se inicialicen los addressables.
        yield return loInitHandle;

        //<< Revisamos que Addressables se inicializó correctamente.
        if (loInitHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loInitHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        //<< Podemos liberar el handle ya que obtuvimos el resultado.
        Addressables.Release(loInitHandle);
        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Se llama para descargar el catalogo.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadCatalog(CoroutineResult loCoroutineResult)
    {
        //<< Generamos la direccion del catalogo de addressables.
        string lcCatalogURL = $"{WebsiteURL}{GetPlatformPath()}/catalog_WLT.bin";

        //<< Indicamos que no se busque en StreamingAssets si no que use la URL como base.
        AddressablesRuntimeProperties.SetPropertyValue("RemoteCatalogProvider", lcCatalogURL);

        //<< Cargamos los datos del catalogo de Addressables.
        goCatalogHandle = Addressables.LoadContentCatalogAsync(lcCatalogURL, false);
        yield return goCatalogHandle;

        //<< Revisamos si se logró obtener el catalogo.
        if (goCatalogHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(goCatalogHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        //<< Intentamos la descarga los shaders compartidos.
        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Carga de Textures2D.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadGameTextures2D(CoroutineResult loCoroutineResult)
    {
        //<< Obtenemos las locaciones de las texturas.
        string[] loKeys = new string[] { nameof(GameTexture2D) };
        AsyncOperationHandle<IList<IResourceLocation>> loLocationsHandle = Addressables.LoadResourceLocationsAsync(loKeys, Addressables.MergeMode.Intersection, typeof(Texture2D));
        yield return loLocationsHandle;

        if (loLocationsHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loLocationsHandle);
            loCoroutineResult.Failed();
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
            loCoroutineResult.Failed();
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
            loSplitString = loLocations[i].PrimaryKey.Split("&");
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

        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Carga de los "UnitAssetBundles" para obtener las UnitInstances.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadUnitAssetBundles_UnitInstance(CoroutineResult loCoroutineResult)
    {
        //<< Obtenemos las locaciones de las texturas.
        string[] loKeys = new string[] { nameof(UnitInstance) };
        AsyncOperationHandle<IList<IResourceLocation>> loLocationsHandle = Addressables.LoadResourceLocationsAsync(loKeys, Addressables.MergeMode.Union);
        yield return loLocationsHandle;

        if (loLocationsHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loLocationsHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        //<< Obtenemos referencia de las locaciones.
        IList<IResourceLocation> loLocations = loLocationsHandle.Result;

        //<< Obtenemos las texturas.
        goUnitInstancesHandle = Addressables.LoadAssetsAsync<GameObject>(loLocations, null);

        yield return goUnitInstancesHandle;

        //<< Si no se logró reintentamos.
        if (goUnitInstancesHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(loLocationsHandle);
            Addressables.Release(goUnitInstancesHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        //<< Las locaciones y UnitInstances estan "emparejados".
        string[] loSplitString;
        int lnUnitAssetBundleId;
        UnitAssetBundleType lnUnitAssetBundleType;
        UnitFaction lnUnitFaction;
        UnitRole lnUnitRole;
        string lcAttachTransformName;
        int lnCount = loLocations.Count;
        for (int i = 0; i < lnCount; i++)
        {
            loSplitString = loLocations[i].PrimaryKey.Split("&");
            if (loSplitString.Length < 5) continue;

            //<< Obtenemos los datos del UnitAssetBundle.
            if (!int.TryParse(loSplitString[0], out lnUnitAssetBundleId)) continue;
            if (!Enum.TryParse(loSplitString[1], out lnUnitAssetBundleType)) continue;
            if (!Enum.TryParse(loSplitString[2], out lnUnitFaction)) continue;
            if (!Enum.TryParse(loSplitString[3], out lnUnitRole)) continue;
            lcAttachTransformName = loSplitString[4];
            if (lcAttachTransformName == "@@@@") lcAttachTransformName = string.Empty;

            //<< Creamos el AssetBundle.
            UnitAssetBundle loUnitAssetBundle = new UnitAssetBundle();
            loUnitAssetBundle.Type = lnUnitAssetBundleType;
            loUnitAssetBundle.Faction = lnUnitFaction;
            loUnitAssetBundle.Role = lnUnitRole;
            loUnitAssetBundle.AttachTransformName = lcAttachTransformName;
            loUnitAssetBundle.unitInstance = goUnitInstancesHandle.Result[i].GetComponentInChildren<UnitInstance>();

            //<< Agregamos el UnitInstance al asset bundle de id dado.
            goUnitAssetBundles.Add(lnUnitAssetBundleId, loUnitAssetBundle);
        }

        //<< Liberamos handle de las locaciones.
        Addressables.Release(loLocationsHandle);

        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Carga de los "UnitAssetBundles" solo para los unitElements.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadUnitAssetBundles_UnitElement(CoroutineResult loCoroutineResult)
    {
        //<< Obtenemos las locaciones de las texturas.
        string[] loKeys = new string[] { nameof(UnitElement) };
        AsyncOperationHandle<IList<IResourceLocation>> loLocationsHandle = Addressables.LoadResourceLocationsAsync(loKeys, Addressables.MergeMode.Union);
        yield return loLocationsHandle;

        if (loLocationsHandle.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(loLocationsHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        //<< Obtenemos referencia de las locaciones.
        IList<IResourceLocation> loLocations = loLocationsHandle.Result;

        //<< Obtenemos las texturas.
        goUnitElementsHandle = Addressables.LoadAssetsAsync<GameObject>(loLocations, null);

        yield return goUnitElementsHandle;

        //<< Si no se logró reintentamos.
        if (goUnitElementsHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(loLocationsHandle);
            Addressables.Release(goUnitElementsHandle);
            loCoroutineResult.Failed();
            yield break;
        }

        //<< Las locaciones y UnitElements estan "emparejados".
        string[] loSplitString;
        int lnUnitAssetBundleId;
        UnitAssetBundleType lnUnitAssetBundleType;
        UnitFaction lnUnitFaction;
        UnitRole lnUnitRole;
        string lcAttachTransformName;
        int lnCount = loLocations.Count;
        for (int i = 0; i < lnCount; i++)
        {
            loSplitString = loLocations[i].PrimaryKey.Split("&");
            if (loSplitString.Length < 5) continue;

            //<< 5_Bow_Any_Archer_RightHand_End
            //(Colors)Enum.Parse(typeof(Colors), "Red, Yellow");

            //<< Obtenemos los datos del UnitAssetBundle.
            if (!int.TryParse(loSplitString[0], out lnUnitAssetBundleId)) continue;
            if (!Enum.TryParse(loSplitString[1], out lnUnitAssetBundleType)) continue;
            if (!Enum.TryParse(loSplitString[2], out lnUnitFaction)) continue;
            if (!Enum.TryParse(loSplitString[3], out lnUnitRole)) continue;
            lcAttachTransformName = loSplitString[4];
            if (lcAttachTransformName == "@@@@") lcAttachTransformName = string.Empty;

            //<< Creamos el AssetBundle.
            UnitAssetBundle loUnitAssetBundle = new UnitAssetBundle();
            loUnitAssetBundle.Type = lnUnitAssetBundleType;
            loUnitAssetBundle.Faction = lnUnitFaction;
            loUnitAssetBundle.Role = lnUnitRole;
            loUnitAssetBundle.AttachTransformName = lcAttachTransformName;
            loUnitAssetBundle.unitElement = goUnitElementsHandle.Result[i].GetComponentInChildren<UnitElement>();

            //<< Agregamos el UnitInstance al asset bundle de id dado.
            goUnitAssetBundles.Add(lnUnitAssetBundleId, loUnitAssetBundle);
        }

        //<< Liberamos handle de las locaciones.
        Addressables.Release(loLocationsHandle);

        loCoroutineResult.Succeed();
        yield break;
    }

    /// <summary>
    /// Se llama en GameManager.
    /// </summary>
    private void OnDestroyAddressables()
    {
        if (goGameTextures2DHandle.IsValid()) Addressables.Release(goGameTextures2DHandle);
        if (goUnitInstancesHandle.IsValid()) Addressables.Release(goUnitInstancesHandle);
        if (goUnitElementsHandle.IsValid()) Addressables.Release(goUnitElementsHandle);
        if (goCatalogHandle.IsValid()) Addressables.Release(goCatalogHandle);
    }
}