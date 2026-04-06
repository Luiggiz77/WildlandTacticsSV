using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Manifesto de los assetboundles.
    /// </summary>
    private static AssetBundleManifest goAssetBundleManifest;

    /// <summary>
    /// Son nuestros asset bundles descargados de los items.
    /// </summary>
    private static Dictionary<string, AssetBundleContent> goItemsAssetBoundleContent = new Dictionary<string, AssetBundleContent>();

    /// <summary>
    /// Indica el nombre del manifest.
    /// </summary>
    private static string gcAssetBundlesRootPath;

    /// <summary>
    /// Debe ir en el start.
    /// </summary>
    private void StartAssetBoundles()
    {
        StartCoroutine(InitializeManifest());
    }

    /// <summary>
    /// Nos da la convercion del id al assetbundle.
    /// </summary>
    /// <param name="lnAssetBundleId"></param>
    /// <returns></returns>
    private static string GetAssetBundleName(int lnAssetBundleId)
    {
        return $"{gcAssetBundlesRootPath}{lnAssetBundleId}";
    }

    /// <summary>
    /// Descargamos e inicializamos el manifest.
    /// </summary>
    /// <param name="lcAssetBundlesRootPath"></param>
    /// <returns></returns>
    private static IEnumerator InitializeManifest(string lcAssetBundlesRootPath = "sai")
    {
        string lcPlatform = Application.platform.ToString().ToLower();
        gcAssetBundlesRootPath = lcAssetBundlesRootPath;

#if UNITY_EDITOR
        if (Application.platform == RuntimePlatform.WindowsEditor) lcPlatform = "android";
#endif

        string lcStreamingAssetsUrl = $"{gcWebURL}sa/{lcPlatform}/{gcAssetBundlesRootPath}/{gcAssetBundlesRootPath}.txt?rnd={Random.Range(0, 77777)}";
        string lcLocalBundlePath = $"{Application.persistentDataPath}/{gcAssetBundlesRootPath}";

#if UNITY_EDITOR
        lcLocalBundlePath = $"{Application.persistentDataPath}/{Unity.Multiplayer.PlayMode.CurrentPlayer.Tags[0]}/{gcAssetBundlesRootPath}";
#endif

        //<< Checamos si el directorio no existe, de ser as� lo creamos.
        if (!Directory.Exists(lcLocalBundlePath)) Directory.CreateDirectory(lcLocalBundlePath);

        lcLocalBundlePath = $"{lcLocalBundlePath}/{gcAssetBundlesRootPath}.txt";

        WaitForSeconds loWaitForSeconds = new WaitForSeconds(3.0f);

        //<< Descargamos si o si el boundle global siempre ya que es necesario y debe ser el m�s actual.
        while (true)
        {
            //<< Descargar el AssetBundle contenedor del manifest.
            using (UnityWebRequest loUnityWebRequest = UnityWebRequest.Get(lcStreamingAssetsUrl))
            {
                yield return loUnityWebRequest.SendWebRequest();

                if (loUnityWebRequest.result != UnityWebRequest.Result.Success)
                {
#if UNITY_EDITOR
                    Debug.Log("Error al descargar manifest bundle: " + loUnityWebRequest.error);
#endif
                    yield return loWaitForSeconds;
                    continue;
                }

                //<< Guardamos el boundle.
                File.WriteAllBytes(lcLocalBundlePath, loUnityWebRequest.downloadHandler.data);
            }

            break;
        }

        //<< Cargamos el AssetBundle y obtenemos el manifest.
        AssetBundle loAssetBundle = AssetBundle.LoadFromFile(lcLocalBundlePath);
        goAssetBundleManifest = loAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        loAssetBundle.Unload(false);
    }

    /// <summary>
    /// Cargamos el assetboundle con base en el Id.
    /// </summary>
    /// <param name="lnAssetBoundle"></param>
    /// <param name="loOnComplete"></param>
    /// <returns></returns>
    private static IEnumerator LoadAssetBundleOld(int lnAssetBundleId, CoroutineResult<string> loCoroutineResult)
    {
        string lcAssetBundleName = GetAssetBundleName(lnAssetBundleId);

        //<< Revisamos si ya lo tenemos en ram.
        if (goItemsAssetBoundleContent.ContainsKey(lcAssetBundleName))
        {
            loCoroutineResult.OnCompleted();
            yield break;
        }

        if (goAssetBundleManifest == null)
        {
            loCoroutineResult.OnFailed("Manifest no inicializado");
            yield break;
        }

        string lcPlatform = Application.platform.ToString().ToLower();
        lcPlatform = "android";

        //<< Descargamos el manifest del item.
        string lcAssetBoundleManifestUrl = $"{gcWebURL}sa/{lcPlatform}/{gcAssetBundlesRootPath}/{lcAssetBundleName}.txt.manifest?rnd={Random.Range(0, 77777)}";
        UnityWebRequest loManifestRequest = UnityWebRequest.Get(lcAssetBoundleManifestUrl);
        yield return loManifestRequest.SendWebRequest();
        if (loManifestRequest.result != UnityWebRequest.Result.Success)
        {
            loCoroutineResult.OnFailed("Error al descargar manifest individual: " + loManifestRequest.error);
            yield break;
        }

        //<< Obtenemos el hash del item del manifest.
        string lcRemoteManifest = loManifestRequest.downloadHandler.text;
        string lcRemoteHash = GetHashFromManifest(lcRemoteManifest);
        string lcLocalHash = PlayerPrefs.GetString($"hash_{lcAssetBundleName}", "");
        string lcLocalBundlePath = $"{Application.persistentDataPath}/{gcAssetBundlesRootPath}/{lcAssetBundleName}.txt";

#if UNITY_EDITOR
        lcLocalBundlePath = $"{Application.persistentDataPath}/{Unity.Multiplayer.PlayMode.CurrentPlayer.Tags[0]}/{gcAssetBundlesRootPath}/{lcAssetBundleName}.txt";
#endif

        //<< Si el archivo existe y tiene el mismo hash cargamos el archivo y lo mandamos en el callback.
        if (File.Exists(lcLocalBundlePath) && lcRemoteHash == lcLocalHash)
        {
            //<< Usar cache local
            AssetBundle loCachedAssetBundle = AssetBundle.LoadFromFile(lcLocalBundlePath);

            //<< Cargamos los datos del asset bundle.
            yield return LoadAssetBundleContent(lnAssetBundleId, loCachedAssetBundle);

            //<< Indicamos que terminamos.
            loCoroutineResult.OnCompleted();
            yield break;
        }

        //<< Descargamos el nuevo assetbundle del item.
        string lcAssetBoundleUrl = $"{gcWebURL}sa/{lcPlatform}/{gcAssetBundlesRootPath}/{lcAssetBundleName}.txt?rnd={Random.Range(0, 77777)}";
        UnityWebRequest loBundleRequest = UnityWebRequest.Get(lcAssetBoundleUrl);
        yield return loBundleRequest.SendWebRequest();
        if (loBundleRequest.result != UnityWebRequest.Result.Success)
        {
            loCoroutineResult.OnFailed("Error al descargar bundle: " + loBundleRequest.error);
            yield break;
        }

        //<< Guardamos el asset boundle en local.
        File.WriteAllBytes(lcLocalBundlePath, loBundleRequest.downloadHandler.data);
        PlayerPrefs.SetString($"hash_{lcAssetBundleName}", lcRemoteHash);
        PlayerPrefs.Save();

        AssetBundle loDownloadedBundle = AssetBundle.LoadFromFile(lcLocalBundlePath);

        //<< Cargamos los datos del asset bundle.
        yield return LoadAssetBundleContent(lnAssetBundleId, loDownloadedBundle);

        //<< Indicamos que terminamos.
        loCoroutineResult.OnCompleted();
    }


    private static IEnumerator LoadAssetBundle(int lnAssetBundleId, CoroutineResult<string> loCoroutineResult)
    {
        string lcAssetBundleName = GetAssetBundleName(lnAssetBundleId);

        //<< Ya cargado en RAM
        if (goItemsAssetBoundleContent.ContainsKey(lcAssetBundleName)) { loCoroutineResult.OnCompleted(); yield break; }

        if (goAssetBundleManifest == null) { loCoroutineResult.OnFailed("Manifest no inicializado"); yield break; }

        string lcPlatform = Application.platform.ToString().ToLower();
        lcPlatform = "android";

        //<< Cargamos de dependencias primero.
        string[] loDependencies = goAssetBundleManifest.GetAllDependencies(lcAssetBundleName);
        foreach (string lcDependencie in loDependencies)
        {
            if (!goItemsAssetBoundleContent.ContainsKey(lcDependencie))
            {
                yield return LoadAssetBundle(lnAssetBundleId, loCoroutineResult);
                if (!loCoroutineResult.Completed) yield break;
            }
        }

        //<< Obtenemos el hash real.
        Hash128 loHash128 = goAssetBundleManifest.GetAssetBundleHash(lcAssetBundleName+".txt");

        string lcUrl = $"{gcWebURL}sa/{lcPlatform}/{gcAssetBundlesRootPath}/{lcAssetBundleName}.txt";

        //<< Descargamos usando cache de Unity
        using (UnityWebRequest loUnityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(lcUrl, loHash128, 0))
        {
            yield return loUnityWebRequest.SendWebRequest();

            if (loUnityWebRequest.result != UnityWebRequest.Result.Success)
            {
                loCoroutineResult.OnFailed("Error al descargar bundle: " + loUnityWebRequest.error);
                yield break;
            }

            AssetBundle loAssetBundle = DownloadHandlerAssetBundle.GetContent(loUnityWebRequest);

            if (loAssetBundle == null)
            {
                loCoroutineResult.OnFailed("Bundle nulo");
                yield break;
            }

            //<< Cargamos contenido
            yield return LoadAssetBundleContent(lnAssetBundleId, loAssetBundle);
        }

        loCoroutineResult.OnCompleted();
    }

    /// <summary>
    /// Obtenemos el hash del asset boundle manifest.
    /// </summary>
    /// <param name="manifestText"></param>
    /// <returns></returns>
    private static string GetHashFromManifest(string manifestText)
    {
        using StringReader reader = new StringReader(manifestText);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Trim().StartsWith("Hash:"))
                return line.Split(':')[1].Trim();
        }
        return string.Empty;
    }

    private static string[] GetDependencies(string bundleName)
    {
        return goAssetBundleManifest.GetAllDependencies(bundleName);
    }

    /// <summary>
    /// Lee el prefab del archivo y lo pasa al resultado.
    /// </summary>
    /// <param name="lcDevicePath"></param>
    /// <param name="loNeonatoInfo"></param>
    /// <param name="OnComplete"></param>
    /// <returns></returns>
    private static IEnumerator LoadAssetBundleContent(int lnAssetBundleId, AssetBundle loAssetBundle)
    {
        string lcAssetBundleName = GetAssetBundleName(lnAssetBundleId);

        AssetBundleContent loAssetBundleContent = new AssetBundleContent(loAssetBundle);
        goItemsAssetBoundleContent.Add(lcAssetBundleName, loAssetBundleContent);

        AssetBundleRequest loAssetBundleRequest = loAssetBundle.LoadAllAssetsAsync();
        yield return loAssetBundleRequest;

        foreach (var loAsset in loAssetBundleRequest.allAssets)
        {
            if (loAsset is GameObject loGameObject)
            {
                loGameObject.name = loAsset.name;
                loAssetBundleContent.GameObjects.Add(loGameObject);
            }
            else if (loAsset is Texture2D loTexture2D)
            {
                loTexture2D.name = loAsset.name;
                loAssetBundleContent.Textures2D.Add(loTexture2D);
            }
            else if (loAsset is Material loMaterial)
            {
                loMaterial.shader = Shader.Find(loMaterial.shader.name);
                loMaterial.name = loAsset.name;
                loAssetBundleContent.Materials.Add(loMaterial);
            }
            else if (loAsset is AnimationClip loAnimationClip)
            {
                loAnimationClip.name = loAsset.name;
                loAssetBundleContent.AnimationClips.Add(loAnimationClip);
            }
        }

        yield break;
    }

    /// <summary>
    /// Se llama para liberar todos los bundles y sus datos.
    /// </summary>
    private static void CleanAssetBundles()
    {
        IEnumerable<string> loKeys = goItemsAssetBoundleContent.Keys;
        int lnCount = loKeys.Count();
        for (int i = lnCount - 1; i >= 0; i--) UnloadAssetBundle(loKeys.ElementAt(i));

        //<< Forzar recolecci�n de basura y recursos (opcional pero �til en editor)
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Se llama para liberar un solo assetbundle.
    /// </summary>
    /// <param name="lnAssetBundleId"></param>
    private static void UnloadAssetBundle(int lnAssetBundleId)
    {
        string lcAssetBundleName = GetAssetBundleName(lnAssetBundleId);

        UnloadAssetBundle(lcAssetBundleName);
    }

    /// <summary>
    /// Se llama para liberar un solo assetbundle.
    /// </summary>
    /// <param name="lcAssetBundleId"></param>
    private static void UnloadAssetBundle(string lcAssetBundleId)
    {
        if (!goItemsAssetBoundleContent.TryGetValue(lcAssetBundleId, out AssetBundleContent loAssetBundleContent)) return;

        //<< Debemos no tener gameobjects.
        Dictionary<GameObject, List<GameObject>> loCloneDictionary;
        foreach (KeyValuePair<string, Dictionary<GameObject, List<GameObject>>> loKeyValuePair in loAssetBundleContent.Clones)
        {
            loCloneDictionary = loKeyValuePair.Value;
            foreach (var loItem in loCloneDictionary)
            {
                foreach (var loClone in loItem.Value) Destroy(loClone);
                loItem.Value.Clear();
            }
            loCloneDictionary.Clear();
        }
        loAssetBundleContent.Clones.Clear();

        // Liberar manualmente referencias si es necesario
        loAssetBundleContent.Textures2D.Clear();
        loAssetBundleContent.Materials.Clear();

        foreach (var loItem in loAssetBundleContent.ClonedSkinnedMeshRenderers)
        {
            foreach (var loSkinnedMeshRenderer in loItem.Value)
            {
                Destroy(loSkinnedMeshRenderer.sharedMesh);
                Destroy(loSkinnedMeshRenderer);
            }
            loItem.Value.Clear();
        }
        loAssetBundleContent.ClonedSkinnedMeshRenderers.Clear();

        loAssetBundleContent.AnimationClips.Clear();

        loAssetBundleContent.GameObjects.Clear();

        // Unload el AssetBundle
        if (loAssetBundleContent.AssetBundle != null)
        {
            loAssetBundleContent.AssetBundle.Unload(true);
            loAssetBundleContent.AssetBundle = null;
        }

        // Remover del diccionario
        goItemsAssetBoundleContent.Remove(lcAssetBundleId);
    }

    /// <summary>
    /// Busca un elemento desactivado o Clona nuestro elemento y lo agrega a la lista de clones.
    /// </summary>
    /// <param name="loModelItemStored"></param>
    /// <param name="loParent"></param>
    /// <returns></returns>
    private static GameObject CreateClone(AssetBundleContent loAssetBundleContent, GameObject loOriginal, Transform loParent, string lcKey)
    {
#if UNITY_EDITOR
        if (!loAssetBundleContent.GameObjects.Contains(loOriginal))
        {
            Debug.LogError($"Este gameobject no puede ser clonado por que no pertenece al assetbundle. GO: {loOriginal.name}");
            return null;
        }
#endif

        //<< Intentamos obtener las direcciones para meter los clones.
        List<GameObject> loCloneList;
        if (!loAssetBundleContent.Clones.TryGetValue(lcKey, out Dictionary<GameObject, List<GameObject>> loCloneDictionary))
        {
            loCloneDictionary = new Dictionary<GameObject, List<GameObject>>();
            loAssetBundleContent.Clones.Add(lcKey, loCloneDictionary);
        }

        if (!loCloneDictionary.TryGetValue(loOriginal, out loCloneList))
        {
            loCloneList = new List<GameObject>();
            loCloneDictionary.Add(loOriginal, loCloneList);
        }

        //<< Como no encontramos el elemento lo clonamos.
        GameObject loGameObject = Instantiate(loOriginal, loParent, false);
        loGameObject.transform.localPosition = Vector3.zero;
        loGameObject.transform.localRotation = Quaternion.identity;
        loGameObject.transform.localScale = Vector3.one;//<< Para que la escala siempre sea 1.
        loCloneList.Add(loGameObject);
        return loGameObject;
    }

    /// <summary>
    /// Remueve los elementos necesario de los objetos activos.
    /// </summary>
    /// <param name="loModelItemStoredLive"></param>
    private static void DestroyClone(int lnAssetBundleId, string lcKey)
    {
        string lcAssetBundleName = GetAssetBundleName(lnAssetBundleId);

        if (!goItemsAssetBoundleContent.TryGetValue(lcAssetBundleName, out AssetBundleContent loAssetBundleContent)) return;

        //<< Liberamos las meshes creadas.
        if (loAssetBundleContent.ClonedSkinnedMeshRenderers.TryGetValue(lcKey, out List<SkinnedMeshRenderer> loClones))
        {
            GameObject loGameObject;
            foreach (SkinnedMeshRenderer loSkinnedMeshRenderer in loClones)
            {
                loGameObject = loSkinnedMeshRenderer.gameObject;
                Destroy(loSkinnedMeshRenderer.sharedMesh);
                Destroy(loSkinnedMeshRenderer);
                Destroy(loGameObject);
            }
            loClones.Clear();
            loAssetBundleContent.ClonedSkinnedMeshRenderers.Remove(lcKey);
        }

        //<< Liberamos el objeto clonado.
        if (loAssetBundleContent.Clones.TryGetValue(lcKey, out Dictionary<GameObject, List<GameObject>> loGameObjectClones))
        {
            foreach (KeyValuePair<GameObject, List<GameObject>> loCloneDictionary in loGameObjectClones)
            {
                foreach (GameObject loClone in loCloneDictionary.Value)
                {
                    if (loClone == null) continue;
                    loClone.SetActive(false);
                    Destroy(loClone);
                }
            }
            loGameObjectClones.Clear();
            loAssetBundleContent.Clones.Remove(lcKey);
        }

        //<< Como si estamos en la lista y solo hay un elemento significa que debemos descargar todos los elementos live.
        if (loAssetBundleContent.Clones.Count > 0) return;

        //<< Destruimos el objeto y lo nulamos.
        UnloadAssetBundle(lnAssetBundleId);
    }
}