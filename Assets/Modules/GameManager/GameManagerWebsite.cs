using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Url para hacer login.
    /// </summary>
    internal static readonly Uri UriLogin = new Uri(UriWebsite, "login");

    /// <summary>
    /// Url para pedir el lenguaje.
    /// </summary>
    internal static readonly Uri UriLanguage = new Uri(UriWebsite, "language");

    /// <summary>
    /// Url para pedir los ids de nuestras unidades.
    /// </summary>
    internal static readonly Uri UriUnitsIds = new Uri(UriWebsite, "unitsids");

    /// <summary>
    /// Llaves de los player prefs.
    /// </summary>
    private const string ClientIdKey = "ClientId";
    private const string PasswordKey = "Password";

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    void AwakeWebsite()
    {
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
                    GameManager.Run(GetObjectDTO<int[]>(UriUnitsIds, loCallback), nameof(GameCommand.GetUnitIds));
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
}