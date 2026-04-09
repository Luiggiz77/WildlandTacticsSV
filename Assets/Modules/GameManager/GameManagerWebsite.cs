using System;
using System.Collections;
using System.Text;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Url del sitio.
    /// </summary>
    private const string gcWebURL = "https://localhost:7049/";

    /// <summary>
    /// Url para hacer login.
    /// </summary>
    internal const string gcUrlLogin = gcWebURL + "login";

    /// <summary>
    /// Llaves de los player prefs.
    /// </summary>
    private const string ClientKey = "Client_Id";
    private const string PasswordKey = "Password_Hash";

    /// <summary>
    /// Tokens
    /// </summary>
    private static string Token { get; set; } = null;
    private static long TokenTimeSpan { get; set; } = 0;

    /// <summary>
    /// Start
    /// </summary>
    private void StartAuth()
    {
        string lcClientId = PlayerPrefs.GetString(ClientKey, Guid.NewGuid().ToString());
        string lcPassword = PlayerPrefs.GetString(PasswordKey, string.Empty);
        if (string.IsNullOrEmpty(lcPassword))
        {
            lcPassword = GeneratePasswordHash(lcClientId);
            PlayerPrefs.SetString(ClientKey, lcClientId);
            PlayerPrefs.SetString(PasswordKey, lcPassword);
        }
    }

    /// <summary>
    /// Nos indica si tenemos un token valido.
    /// </summary>
    /// <returns></returns>
    private static bool IsTokenValid()
    {
        if (string.IsNullOrWhiteSpace(Token)) return false;
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < TokenTimeSpan;
    }

    /// <summary>
    /// Nos da el ClientId.
    /// </summary>
    /// <returns></returns>
    private static string ClientId()
    {
        return PlayerPrefs.GetString(ClientKey);
    }

    /// <summary>
    /// Nos da el passwordhash.
    /// </summary>
    /// <returns></returns>
    private static string PasswordHash()
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
        DTO2<string, long> loResponse = JsonUtility.FromJson<DTO2<string, long>>(lcResult);
        Token = loResponse.Item1;
        TokenTimeSpan = loResponse.Item2;
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
            yield return Post(gcUrlLogin, OnWebLogin, null, new DTO2<string, string>(nameof(ClientId), ClientId()), new DTO2<string, string>(nameof(PasswordHash), PasswordHash()));
            if (!IsTokenValid()) yield return loWaitForSeconds;
        }
        yield break;
    }
}