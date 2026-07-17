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
    /// Url para obtener las constantes del tablero.
    /// </summary>
    private static Uri UriBoardConstants = null;

    /// <summary>
    /// Url para obtener los tableros de distribución.
    /// </summary>
    private static Uri UriDistributionBoards = null;

    /// <summary>
    /// Url para obtener las coordenadas de las unidades en los tableros de distribución.
    /// </summary>
    private static Uri UriDistributionBoardsUnitCoordinates = null;

    /// <summary>
    /// Url para obtener todos los items existentes del juego.
    /// </summary>
    private static Uri UriGameplayItems = null;

    /// <summary>
    /// Url para obtener todos los items del jugador.
    /// </summary>
    private static Uri UriGameplayItemsStored = null;

    /// <summary>
    /// Url para indicar que debemos agregar una unidad al tablero de distribución.
    /// </summary>
    private static Uri UriAddUnitToDistributionBoard = null;

    /// <summary>
    /// Url para indicar que debemos remover una unidad al tablero de distribución.
    /// </summary>
    private static Uri UriRemoveUnitFromDistributionBoard = null;

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
    private BoardConstants goBoardConstants = new BoardConstants() { DistributionBoardLength = 3, Width = 6, MaxUnits = 5, };

    /// <summary>
    /// Nuestros tableros de distribución.
    /// </summary>
    private Dictionary<int, DistributionBoard> goDistributionBoards = new Dictionary<int, DistributionBoard>();

    /// <summary>
    /// Lista de items del juego.
    /// </summary>
    private Dictionary<int, GameplayItem> goGameplayItems = new Dictionary<int, GameplayItem>();

    /// <summary>
    /// Inventario de items del usuario.
    /// </summary>
    private IEnumerable<GameplayItemStored> goInventory = Array.Empty<GameplayItemStored>();

    /// <summary>
    /// Nos indica el tablero de distribución seleccionado.
    /// </summary>
    private static int gnSelectedDistributionBoardId = 0;

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    private void AwakeWebsite()
    {
        UriWebsite = new Uri(WebsiteURL);
        UriLogin = new Uri(UriWebsite, "login");
        UriLanguage = new Uri(UriWebsite, "language");
        UriUnitsProperties = new Uri(UriWebsite, "UnitsProperties");
        UriUnitsBattleProperties = new Uri(UriWebsite, "UnitsBattleProperties");
        UriBoardConstants = new Uri(UriWebsite, "BoardConstants");
        UriDistributionBoards = new Uri(UriWebsite, "DistributionBoards");
        UriDistributionBoardsUnitCoordinates = new Uri(UriWebsite, "DistributionBoardsUnitCoordinates");
        UriAddUnitToDistributionBoard = new Uri(UriWebsite, "AddUnitToDistributionBoard");
        UriRemoveUnitFromDistributionBoard = new Uri(UriWebsite, "RemoveUnitFromDistributionBoard");
        UriGameplayItems = new Uri(UriWebsite, "GameplayItems");
        UriGameplayItemsStored = new Uri(UriWebsite, "GameplayItemsStored");

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

        //<< Pedimos los objetos.
        GameManager.Run(GetObjectDTO<GameplayItem[]>(UriGameplayItems, OnGetGameplayItems), nameof(UriGameplayItems));

        //<< Pedimos nuestros objetos.
        GameManager.Run(GetObjectDTO<GameplayItemStored[]>(UriGameplayItemsStored, OnGetGameplayItemsStored), nameof(UriGameplayItemsStored));
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
        foreach (DistributionBoard loDistributionBoard in loDistributionBoards)
        {
            loDistributionBoard.Setup(goBoardConstants.Width, goBoardConstants.DistributionBoardLength);
            goDistributionBoards.Add(loDistributionBoard.Id, loDistributionBoard);
        }

        //<< Indicamos cual es el primer tablero de distribución seleccionado.
        gnSelectedDistributionBoardId = goDistributionBoards.First().Key;

        //<< Pedimos las unidades y sus coordenadas de los tableros de distribución.
        GameManager.Run(GetObjectDTO<DistributionBoardUnitCoordinates[]>(UriDistributionBoardsUnitCoordinates, OnGetDistributionBoardsUnitCoordinates), nameof(UriDistributionBoardsUnitCoordinates));
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
    /// Obtenemos los objetos del juego.
    /// </summary>
    private void OnGetGameplayItems(GameplayItem[] loGameplayItems)
    {
        foreach (GameplayItem loGameplayItem in loGameplayItems) goGameplayItems.Add(loGameplayItem.Id, loGameplayItem);
    }

    /// <summary>
    /// Obtenemos los objetos del jgador.
    /// </summary>
    private void OnGetGameplayItemsStored(GameplayItemStored[] loGameplayItemsStored)
    {
        goInventory = loGameplayItemsStored;
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
                    loCallback.Invoke(goBoardConstants.Width, goBoardConstants.DistributionBoardLength);
                }
                break;

            case GameCommand.GetDistributionBoards:
                {
                    UnityAction<IEnumerable<DistributionBoard>> loCallback = (UnityAction<IEnumerable<DistributionBoard>>)loParams[0];
                    loCallback.Invoke(goDistributionBoards.Values);
                }
                break;

            case GameCommand.UnitExistsOnDistributionBoard: UnitExistsOnDistributionBoard((int)loParams[0], (int)loParams[1], (UnityAction<bool>)loParams[1]); break;
            case GameCommand.AddUnitToDistributionBoard: Run(AddUnitToDistributionBoard((int)loParams[0], (int)loParams[1], (int)loParams[2], (int)loParams[3]), nameof(GameCommand.AddUnitToDistributionBoard)); break;
            case GameCommand.RemoveUnitFromDistributionBoard: Run(RemoveUnitFromDistributionBoard((int)loParams[0], (int)loParams[1]), nameof(GameCommand.RemoveUnitFromDistributionBoard)); break;

            case GameCommand.GetGameplayItemsStored: GetGameplayItemsStored((GameplayItemTiming?)loParams[0], (UnityAction<IEnumerable<GameplayItemStored>>)loParams[1]); break;

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
    /// Se llama para hacer login forzado hasta que se logre. (Forzado)
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
    /// Se llama para hacer login forzado hasta que se logre con control de error.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator Login(CoroutineResult<string> loResult)
    {
        //<< Revisamos si hay internet.
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            loResult.OnFailed($"{nameof(Application.internetReachability)}: {nameof(NetworkReachability.NotReachable)}");
            yield break;
        }

        //<< Si el token es valido y tenemos internet continuamos correctamente.
        if (IsTokenValid())
        {
            loResult.OnCompleted();
            yield break;
        }

        //<< Revisamos si podemos hacer login.
        yield return Post(UriLogin, OnWebLogin, null, new DTO2<string, string>(nameof(ClientId), ClientId()), new DTO2<string, string>(nameof(Password), Password()));

        //<< Revisamos si el token es valido.
        if (!IsTokenValid())
        {
            loResult.OnFailed($"Token invalid.");
            yield break;
        }

        loResult.OnCompleted();
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

    #region GAME
    /// <summary>
    /// Nos indica si la unidad se encuentra en algún mapa de distribución.
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    /// <param name="loUnitExistsOnDistributionBoard"></param>
    private void UnitExistsOnDistributionBoard(int lnDistributionBoardId, int lnUnitPropertiesId, UnityAction<bool> loUnitExistsOnDistributionBoard)
    {
        loUnitExistsOnDistributionBoard.Invoke(goDistributionBoards[lnDistributionBoardId].Contains(lnUnitPropertiesId));
    }

    /// <summary>
    /// Para establecer una unidad en el tablero de distribución.
    /// </summary>
    /// <param name="lnId"></param>
    /// <returns></returns>
    private IEnumerator AddUnitToDistributionBoard(int lnDistributionBoardId, int lnUnitPropertiesId, int lnDistributionBoardX, int lnDistributionBoardZ)
    {
        //<< Revisamos si tenemos login.
        CoroutineResult<string> loCoroutineResult = new CoroutineResult<string>();
        yield return Login(loCoroutineResult);
        if (!loCoroutineResult.Completed)
        {
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.NoInternet), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Ok), null, null);
            yield break;
        }

        //<< Obtenemos el tablero de distribución.
        DistributionBoard loDistributionBoard = goDistributionBoards[lnDistributionBoardId];

        //<< Revisamos si en la posicion hay otra unidad.
        int lnUnitPropertiesIdSwitch = loDistributionBoard.GetIdOf(lnDistributionBoardX, lnDistributionBoardZ);

        //<< Si es la misma unidad en el mismo lugar no hacemos nada.
        if (lnUnitPropertiesIdSwitch == lnUnitPropertiesId) yield break;

        //<< Revisamos si nos pasariamos del maximo de unidades permitidas.
        if (loDistributionBoard.GetUnitsCount() + 1 > goBoardConstants.MaxUnits && lnUnitPropertiesIdSwitch <= 0)
        {
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.MaxUnits).Replace(TextTags.MaxUnits, $"{goBoardConstants.MaxUnits}"), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Ok), null, null);
            yield break;
        }

        //<< Ya que llegamos aqui pedimos al servidor que haga la actualización del tablero de distribución.
        yield return Post(UriAddUnitToDistributionBoard, null, loCoroutineResult, new DTO2<string, string>($"DistributionBoardId", $"{lnDistributionBoardId}"), new DTO2<string, string>($"UnitPropertiesId", $"{lnUnitPropertiesId}"), new DTO2<string, string>($"DistributionBoardX", $"{lnDistributionBoardX}"), new DTO2<string, string>($"DistributionBoardZ", $"{lnDistributionBoardZ}"));
        if (!loCoroutineResult.Completed)
        {
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.NoInternet), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Ok), null, null);
            yield break;
        }

        //<< Ya que se guardó el cambio en servidor lo hacemos en local.

        //<< Si hay unidad de intercambio tambien debe removerse del mapa previo a ser agregada.
        if (lnUnitPropertiesIdSwitch > 0) Send(GameCommand.RemovedUnitFromDistributionBoard, lnDistributionBoardId, lnUnitPropertiesIdSwitch);

        //<< Removemos rastro de la unidad del tablero si es que existe en el, de ser así avisamos.
        if (loDistributionBoard.RemoveUnit(lnUnitPropertiesId, out int lnX, out int lnZ)) Send(GameCommand.RemovedUnitFromDistributionBoard, lnDistributionBoardId, lnUnitPropertiesId);

        //<< Agregamos la unidad en la posicion dada.
        loDistributionBoard.AddUnit(lnUnitPropertiesId, lnDistributionBoardX, lnDistributionBoardZ);

        //<< Si habia una unidad previa en el lugar sabemos que debemos intercambiar posiciones entre unidades.
        if (lnUnitPropertiesIdSwitch > 0 && (lnDistributionBoardX != lnX || lnDistributionBoardZ != lnZ))
        {
            //<< Agregamos la unidad del lugar en la posicion original de la otra unidad.
            loDistributionBoard.AddUnit(lnUnitPropertiesIdSwitch, lnX, lnZ);
        }

        //<< Avisamos que se agrego la unidad al tablero de distribución.
        Send(GameCommand.AddedUnitToDistributionBoard, lnDistributionBoardId, lnUnitPropertiesId, lnDistributionBoardX, lnDistributionBoardZ);

        //<< Si habia una unidad previa en el lugar sabemos que debemos intercambiar posiciones entre unidades.
        if (lnUnitPropertiesIdSwitch > 0 && (lnDistributionBoardX != lnX || lnDistributionBoardZ != lnZ))
        {
            //<< Avisamos que se agrego la unidad al tablero de distribución.
            Send(GameCommand.AddedUnitToDistributionBoard, lnDistributionBoardId, lnUnitPropertiesIdSwitch, lnX, lnZ);
        }
    }

    /// <summary>
    /// Para remover una unidad del tablero de distribución.
    /// </summary>
    /// <param name="lnId"></param>
    /// <returns></returns>
    private IEnumerator RemoveUnitFromDistributionBoard(int lnDistributionBoardId, int lnUnitPropertiesId)
    {
        //<< Revisamos si tenemos login.
        CoroutineResult<string> loCoroutineResult = new CoroutineResult<string>();
        yield return Login(loCoroutineResult);
        if (!loCoroutineResult.Completed)
        {
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.NoInternet), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Ok), null, null);
            yield break;
        }

        //<< Obtenemos el tablero de distribución.
        DistributionBoard loDistributionBoard = goDistributionBoards[lnDistributionBoardId];

        //<< Revisamos que contenga la unidad.
        if (!loDistributionBoard.Contains(lnUnitPropertiesId))
        {
            Send(GameCommand.RemovedUnitFromDistributionBoard, lnDistributionBoardId, lnUnitPropertiesId);
            yield break;
        }

        //<< Ya que llegamos aqui pedimos al servidor que haga la actualización del tablero de distribución.
        yield return Post(UriRemoveUnitFromDistributionBoard, null, loCoroutineResult, new DTO2<string, string>($"DistributionBoardId", $"{lnDistributionBoardId}"), new DTO2<string, string>($"UnitPropertiesId", $"{lnUnitPropertiesId}"));
        if (!loCoroutineResult.Completed)
        {
            GameManager.Send(GameCommand.ShowUIModal, GameManager.GetText(GameTextUsage.Error, (int)GameTextError.NoInternet), GameManager.GetText(GameTextUsage.Word, (int)GameTextWord.Ok), null, null);
            yield break;
        }

        //<< Ya que se guardó el cambio en servidor lo hacemos en local.
        loDistributionBoard.RemoveUnit(lnUnitPropertiesId);
        Send(GameCommand.RemovedUnitFromDistributionBoard, lnDistributionBoardId, lnUnitPropertiesId);
        yield break;
    }

    ///// <summary>
    ///// Se llama para remover una unidad del mapa de distribución con base en el id dado. (Temp)
    ///// </summary>
    ///// <param name="lnUnitPropertiesId"></param>
    //private void RemoveUnitFromDistributionBoard(int lnUnitPropertiesId, int lnDistributionBoardId)
    //{
    //    //<< Obtenemos el tablero.
    //    DistributionBoard loDistributionBoard;

    //    UnitProperties loUnitProperties = GetUnitProperties(lnUnitPropertiesId);
    //    if (loUnitProperties.Opponent) loDistributionBoard = goDistributionBoardsOpponent.FirstOrDefault(x => x.DistributionBoardId == lnDistributionBoardId);
    //    else loDistributionBoard = goDistributionBoardsUser.FirstOrDefault(x => x.DistributionBoardId == lnDistributionBoardId);

    //    //<< Removemos rastro de la unidad del tablero si es que existe en el.
    //    loDistributionBoard.RemoveUnit(lnUnitPropertiesId);

    //    //<< Guadamos localmente el tablero de distribución. (Temp) Nota: No se sabe si se guardaran en servidor tambien. FINDME
    //    PlayerPrefsJson.Set($"{(loUnitProperties.Opponent ? PlayerPrefsJson.Key.DistributionBoardOpponent : PlayerPrefsJson.Key.DistributionBoardUser)}{lnDistributionBoardId}", loDistributionBoard);

    //    //<< Avisamos que se ha removido la unidad del tablero de distribución.
    //    Send(GameCommand.RemovedUnitFromDistributionBoard, lnUnitPropertiesId, lnDistributionBoardId);
    //}

    /// <summary>
    /// Se llama para cuando piden item del inventario ya sea completos o filtrados.
    /// </summary>
    /// <param name="lbOpponent"></param>
    /// <param name="loTiming"></param>
    /// <param name="loCallback"></param>
    private void GetGameplayItemsStored(GameplayItemTiming? loTiming, UnityAction<IEnumerable<GameplayItemStored>> loCallback)
    {
        //<< Si lo piden completo.
        if (loTiming == null) { loCallback.Invoke(goInventory); return; }

        //<< Si lo piden filtrado buscamos su timing.
        List<GameplayItemStored> loFiltered = new List<GameplayItemStored>();
        GameplayItem loGameplayItem;
        foreach (GameplayItemStored loItem in goInventory)
        {
            loGameplayItem = goGameplayItems[loItem.GameplayItemId];
            if (loGameplayItem.Timing == loTiming.Value) loFiltered.Add(loItem);
        }

        //<< Mandamos filtrado el inventario.
        loCallback.Invoke(loFiltered);
    }
    #endregion
}