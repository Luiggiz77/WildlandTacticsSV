using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Url del sitio.
    /// </summary>
    private const string WebsiteURL = "https://localhost:7175/";

    /// <summary>
    /// Uri del sitio web.
    /// </summary>
    private static Uri UriWebsite = null;

    /// <summary>
    /// Url para descargar el archivo "version.txt" para adressables.
    /// </summary>
    private static Uri UriVersionFile = null;

    /// <summary>
    /// Url para hacer login.
    /// </summary>
    private static Uri UriLogin = null;

    /// <summary>
    /// Url para pedir el lenguaje.
    /// </summary>
    private static Uri UriLanguage = null;

    /// <summary>
    /// Url para pedir las propiedades de nuestras unidades.
    /// </summary>
    private static Uri UriUnitsProperties = null;

    /// <summary>
    /// Url para pedir las propiedades de batalla de nuestras unidades.
    /// </summary>
    private static Uri UriUnitsBattleProperties = null;

    /// <summary>
    /// Url para obtenes las constantes del tablero.
    /// </summary>
    private static Uri UriBoardConstants = null;

    /// <summary>
    /// Url para obtenes los tableros de distribución.
    /// </summary>
    private static Uri UriDistributionBoards = null;

    /// <summary>
    /// Url para obtenes las coordenadas de las unidades en los tableros de distribución.
    /// </summary>
    private static Uri UriDistributionBoardsUnitCoordinates = null;

    /// <summary>
    /// Llaves de los player prefs.
    /// </summary>
    private const string ClientIdKey = "ClientId";
    private const string PasswordKey = "Password";

    /// <summary>
    /// Propiedades de batalla de todas las unidades.
    /// </summary>
    private Dictionary<int, UnitProperties> goUnitsProperties = new Dictionary<int, UnitProperties>();

    /// <summary>
    /// Propiedades de batalla de todas las unidades.
    /// </summary>
    private Dictionary<int, UnitBattleProperties> goUnitsBattleProperties = new Dictionary<int, UnitBattleProperties>();

    /// <summary>
    /// Nuestras constantes de tablero.
    /// </summary>
    private BoardConstants goBoardConstants = new BoardConstants() { DistributionBoardLenght = 3, Widht = 6, MaxUnits = 5, };

    /// <summary>
    /// Nuestros tableros de distribución.
    /// </summary>
    private Dictionary<int, DistributionBoard> goDistributionBoards = new Dictionary<int, DistributionBoard>();

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    void AwakeWebsite()
    {
        UriWebsite = new Uri(WebsiteURL);
        UriLogin = new Uri(UriWebsite, "login");
        UriLanguage = new Uri(UriWebsite, "language");
        UriUnitsProperties = new Uri(UriWebsite, "UnitsProperties");
        UriUnitsBattleProperties = new Uri(UriWebsite, "UnitsBattleProperties");
        UriVersionFile = new Uri(UriWebsite, $"{GetPlatformPath()}/version.txt?rand={UnityEngine.Random.Range(0, int.MaxValue)}");
        UriBoardConstants = new Uri(UriWebsite, "BoardConstants");
        UriDistributionBoards = new Uri(UriWebsite, "DistributionBoards");
        UriDistributionBoardsUnitCoordinates = new Uri(UriWebsite, "DistributionBoardsUnitCoordinates");

        string lcClientId = PlayerPrefs.GetString(ClientIdKey, Guid.NewGuid().ToString());
        string lcPassword = PlayerPrefs.GetString(PasswordKey, string.Empty);
        if (string.IsNullOrEmpty(lcPassword))
        {
            lcPassword = GeneratePasswordHash(lcClientId);
            PlayerPrefs.SetString(ClientIdKey, lcClientId);
            PlayerPrefs.SetString(PasswordKey, lcPassword);
        }

        //<< Nos conectamos a los comandos.
        AddListener(OnGameCommandWebsite);
    }

    /// <summary>
    /// Se llama en el Start del gameManager.
    /// </summary>
    private void StartWebsite()
    {
        //<< Pedimos los UnitBattleProperties.
        GameManager.Run(GetObjectDTO<UnitBattleProperties[]>(UriUnitsBattleProperties, OnGetUnitsBattleProperties), nameof(UriUnitsBattleProperties));

        //<< Pedimos los UnitProperties.
        GameManager.Run(GetObjectDTO<UnitProperties[]>(UriUnitsProperties, OnGetUnitsProperties), nameof(UriUnitsProperties));

        //<< Pedimos las constantes del tablero.
        GameManager.Run(GetObjectDTO<BoardConstants>(UriBoardConstants, OnGetBoardConstants), nameof(UriBoardConstants));

        //<< Pedimos los tableros de distribución.
        GameManager.Run(GetObjectDTO<DistributionBoard[]>(UriDistributionBoards, OnGetDistributionBoards), nameof(UriDistributionBoards));

        //<< Pedimos los tableros de distribución.
        GameManager.Run(GetObjectDTO<DistributionBoardUnitCoordinates[]>(UriDistributionBoardsUnitCoordinates, OnGetDistributionBoardsUnitCoordinates), nameof(UriDistributionBoardsUnitCoordinates));
    }

    /// <summary>
    /// Callback de cuando nos dan las propiedades de las unidades.
    /// </summary>
    private void OnGetUnitsProperties(UnitProperties[] loUnitsProperties)
    {
        foreach (UnitProperties loUnitProperties in loUnitsProperties) goUnitsProperties.Add(loUnitProperties.Id, loUnitProperties);
    }

    /// <summary>
    /// Callback de cuando nos dan las propiedades de batalla de las unidades.
    /// </summary>
    private void OnGetUnitsBattleProperties(UnitBattleProperties[] loUnitsBattleProperties)
    {
        foreach (UnitBattleProperties loUnitBattleProperties in loUnitsBattleProperties) goUnitsBattleProperties.Add(loUnitBattleProperties.Id, loUnitBattleProperties);
    }

    /// <summary>
    /// Obtenemos las constantes del tablero.
    /// </summary>
    private void OnGetBoardConstants(BoardConstants loBoardConstants)
    {
        goBoardConstants = loBoardConstants;
    }

    /// <summary>
    /// Obtenemos los tableros de distribución.
    /// </summary>
    private void OnGetDistributionBoards(DistributionBoard[] loDistributionBoards)
    {
        foreach (DistributionBoard loDistributionBoard in loDistributionBoards) goDistributionBoards.Add(loDistributionBoard.Id, loDistributionBoard);
    }

    /// <summary>
    /// Obtenemos las coordenadas de las unidades en los tableros de distribución.
    /// </summary>
    private void OnGetDistributionBoardsUnitCoordinates(DistributionBoardUnitCoordinates[] loDistributionBoardsUnitCoordinates)
    {
        if (loDistributionBoardsUnitCoordinates == null) return;
        DistributionBoard loDistributionBoard;
        foreach (DistributionBoardUnitCoordinates loUnitCoordinates in loDistributionBoardsUnitCoordinates)
        {
            if (!goDistributionBoards.TryGetValue(loUnitCoordinates.DistributionBoardId, out loDistributionBoard)) continue;
            loDistributionBoard.AddUnit(loUnitCoordinates.UnitPropertiesId, loUnitCoordinates.X, loUnitCoordinates.Z);
        }
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommandWebsite(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.RequestLanguage:
                {
                    UnityAction<bool, string> loCallback = (UnityAction<bool, string>)loParams[0];
                    DTO2<string, string>[] loPostParams = (DTO2<string, string>[])loParams[1];
                    GameManager.Run(GetJson(UriLanguage, loCallback, loPostParams), nameof(GameCommand.RequestLanguage));
                }
                break;

            case GameCommand.GetUnitIds:
                {
                    UnityAction<int[]> loCallback = (UnityAction<int[]>)loParams[0];
                    loCallback.Invoke(goUnitsProperties.Keys.ToArray());
                }
                break;

            case GameCommand.GetUnitProperties:
                {
                    int lnUnitPropertiesId = (int)loParams[0];
                    UnityAction<UnitBattleProperties, UnitProperties> loCallback = (UnityAction<UnitBattleProperties, UnitProperties>)loParams[1];
                    UnitProperties loUnitProperties = goUnitsProperties[lnUnitPropertiesId];
                    UnitBattleProperties loUnitBattleProperties = goUnitsBattleProperties[loUnitProperties.UnitBattlePropertiesId];
                    loCallback.Invoke(loUnitBattleProperties, loUnitProperties);
                }
                break;

            case GameCommand.GetDistributionBoardDimensions:
                {
                    UnityAction<int, int> loCallback = (UnityAction<int, int>)loParams[0];
                    loCallback.Invoke(goBoardConstants.Widht, goBoardConstants.DistributionBoardLenght);
                }
                break;

            case GameCommand.GetDistributionBoards:
                {
                    UnityAction<IEnumerable<DistributionBoard>> loCallback = (UnityAction<IEnumerable<DistributionBoard>>)loParams[0];
                    loCallback.Invoke(goDistributionBoards.Values);
                }
                break;
            default: break;
        }
    }

    /// <summary>
    /// Nos indica si tenemos un token valido.
    /// </summary>
    /// <returns></returns>
    private static bool IsTokenValid()
    {
        if (string.IsNullOrWhiteSpace(Token)) return false;
        return DateTime.UtcNow < TokenExpiration;
    }

    /// <summary>
    /// Nos da el ClientId.
    /// </summary>
    /// <returns></returns>
    private static string ClientId()
    {
        return PlayerPrefs.GetString(ClientIdKey);
    }

    /// <summary>
    /// Nos da el password.
    /// </summary>
    /// <returns></returns>
    private static string Password()
    {
        return PlayerPrefs.GetString(PasswordKey);
    }

    /// <summary>
    /// Nos da el passwordhash.
    /// </summary>
    /// <param name="lcDeviceId"></param>
    /// <returns></returns>
    private string GeneratePasswordHash(string lcDeviceId)
    {
        var lcSalt = SystemInfo.deviceUniqueIdentifier;
        var lcRaw = lcDeviceId + lcSalt;
        var loBytes = Encoding.UTF8.GetBytes(lcRaw);
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(loBytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Callback para darnos los datos del login plus un hash para poder jugar.
    /// </summary>
    /// <param name="lcIP"></param>
    private static void OnWebLogin(bool lbResult, string lcResult)
    {
        if (!lbResult)
        {
            Debug.Log(lcResult);
            return;
        }

        //<< Nos dan el token del usuario para comunicarnos con el backend.
        DTO2<string, double> loResponse = JsonUtility.FromJson<DTO2<string, double>>(lcResult);
        Token = loResponse.Item1;
        TokenExpiration = DateTime.UtcNow.AddSeconds(loResponse.Item2);
    }

    /// <summary>
    /// Para checar si hay internet de lo contrario esperar a que haya internet.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator WaitInternetReachability()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable) yield break;
        WaitForSeconds loWaitForSeconds = new WaitForSeconds(3);
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return loWaitForSeconds;
            continue;
        }
    }

    /// <summary>
    /// Se llama para hacer login forzado hasta que se logre.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator Login()
    {
        WaitForSeconds loWaitForSeconds = new WaitForSeconds(3);
        while (!IsTokenValid())
        {
            yield return WaitInternetReachability();
            yield return Post(UriLogin, OnWebLogin, null, new DTO2<string, string>(nameof(ClientId), ClientId()), new DTO2<string, string>(nameof(Password), Password()));
            if (!IsTokenValid()) yield return loWaitForSeconds;
        }
        yield break;
    }

    /// <summary>
    /// Se llama para pedir un json al sitio web.
    /// </summary>
    /// <param name="lcUrl"></param>
    /// <param name="loCallback"></param>
    /// <returns></returns>
    private static IEnumerator GetJson(Uri loUri, UnityAction<bool, string> loCallback, params DTO2<string, string>[] loParams)
    {
        yield return Login();
        yield return Post(loUri, loCallback, null, loParams);
        yield break;
    }

    /// <summary>
    /// Nos da el objeto con base en el string descargado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loUri"></param>
    /// <param name="loCallback"></param>
    /// <param name="loParams"></param>
    /// <returns></returns>
    private static IEnumerator GetObject<T>(Uri loUri, UnityAction<T> loCallback, params DTO2<string, string>[] loParams) where T : class
    {
        yield return Login();
        CoroutineResult<string> loCoroutineResult = new CoroutineResult<string>();
        yield return Post(loUri, null, loCoroutineResult, loParams);
        if (!loCoroutineResult.Completed) loCallback.Invoke(null);
        else
        {
            string lcJson = loCoroutineResult.Object;
            T loObject = JsonUtility.FromJson<T>(lcJson);
            loCallback.Invoke(loObject);
        }
        yield break;
    }

    /// <summary>
    /// Nos da el objeto pero atraves de un DTO.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loUri"></param>
    /// <param name="loCallback"></param>
    /// <param name="loParams"></param>
    /// <returns></returns>
    private static IEnumerator GetObjectDTO<T>(Uri loUri, UnityAction<T> loCallback, params DTO2<string, string>[] loParams) where T : class
    {
        yield return Login();
        CoroutineResult<string> loCoroutineResult = new CoroutineResult<string>();
        yield return Post(loUri, null, loCoroutineResult, loParams);
        if (!loCoroutineResult.Completed) loCallback.Invoke(null);
        else
        {
            string lcJson = loCoroutineResult.Object;
            DTO1<T> loObject = JsonUtility.FromJson<DTO1<T>>(lcJson);
            loCallback.Invoke(loObject.Item1);
        }
        yield break;
    }


    /// <summary>
    /// Se llama para descargar un archivo de texto.
    /// </summary>
    /// <param name="loUri"></param>
    /// <param name="loResult"></param>
    /// <returns></returns>
    private static IEnumerator DownloadTextFile(Uri loUri, CoroutineResult<string> loResult)
    {
        //<< Creamos la petición GET a la URL
        using (UnityWebRequest loUnityWebRequest = UnityWebRequest.Get(loUri))
        {
            //<< Enviamos la petición y esperamos a que responda
            yield return loUnityWebRequest.SendWebRequest();

            //<< Verificamos si hubo algún error
            if (loUnityWebRequest.result == UnityWebRequest.Result.ConnectionError || loUnityWebRequest.result == UnityWebRequest.Result.ProtocolError) loResult.OnFailed(loUnityWebRequest.error);
            else loResult.OnCompleted(loUnityWebRequest.downloadHandler.text);
        }
    }

    /// <summary>
    /// Nos da parte del path con base en la plataforma.
    /// </summary>
    /// <returns></returns>
    private static string GetPlatformPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer: return "StandaloneWindows64";
            case RuntimePlatform.Android: return "Android";
            case RuntimePlatform.IPhonePlayer: return "iOS";
            case RuntimePlatform.OSXPlayer: return "StandaloneOSX";
            case RuntimePlatform.LinuxPlayer: return "StandaloneLinux64";
            default: return "UnknownPlatform";
        }
    }
}