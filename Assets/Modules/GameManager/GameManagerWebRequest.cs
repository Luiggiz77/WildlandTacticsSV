using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Solo se usa para obtener la llave.
    /// </summary>
    private class PrivateCertificateHandler : CertificateHandler
    {
        /// <summary>
        /// Es la llave publica capturada.
        /// </summary>
        private static string PublicKey = string.Empty;
        protected override bool ValidateCertificate(byte[] loCertificateData)
        {
            X509Certificate2 loCertificate = new X509Certificate2(loCertificateData);
            if (string.IsNullOrWhiteSpace(PublicKey))
            {
                PublicKey = loCertificate.GetPublicKeyString().ToUpper();
                return true;
            }

            string lcPublicKey = loCertificate.GetPublicKeyString().ToUpper();
            if (!lcPublicKey.Equals(PublicKey)) return false;
            return true;
        }
    }

    /// <summary>
    /// Tokens
    /// </summary>
    internal static string Token = null;
    private static long TokenTimeSpan = 0;

    /// <summary>
    /// Son las texturas 2D instanciadas.
    /// </summary>
    private static Dictionary<string, Texture2D> goTextures2D = new Dictionary<string, Texture2D>();

    /// <summary>
    /// Nos da texto de un Get.
    /// </summary>
    public static IEnumerator GetText(string lcURL, UnityAction<bool, string> loOnResult)
    {
        WaitForSeconds loWaitForSeconds = new WaitForSeconds(0.5f);
        using (UnityWebRequest loUnityWebRequest = UnityWebRequest.Get(lcURL))
        {
            if (!string.IsNullOrWhiteSpace(Token)) loUnityWebRequest.SetRequestHeader("Authorization", "Bearer " + Token);

            using (loUnityWebRequest.downloadHandler = new DownloadHandlerBuffer())
            {
                using (loUnityWebRequest.certificateHandler = new PrivateCertificateHandler())
                {
                    yield return loUnityWebRequest.SendWebRequest();

                    //<< Esperamos la descarga completa.
                    while (!loUnityWebRequest.isDone) yield return loWaitForSeconds;

                    //<< Si no hay algun regresamos.
                    if (loUnityWebRequest.result == UnityWebRequest.Result.Success) loOnResult.Invoke(true, loUnityWebRequest.downloadHandler.text);
                    else loOnResult.Invoke(false, string.Empty);

                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// Se usa para enviar cosas al servidor.
    /// </summary>
    /// <param name="lcUrl"></param>
    /// <param name="lcText"></param>
    /// <param name="loOnresult"></param>
    /// <returns></returns>
    public static IEnumerator Post(string lcUrl, UnityAction<bool, string> loOnResult)
    {
        using (UnityWebRequest loUnityWebRequest = new UnityWebRequest(lcUrl, "POST"))
        {
            if (!string.IsNullOrWhiteSpace(Token)) loUnityWebRequest.SetRequestHeader("Authorization", "Bearer " + Token);
            using (loUnityWebRequest.downloadHandler = new DownloadHandlerBuffer())
            {
                loUnityWebRequest.SetRequestHeader("Content-Type", "text/html; charset=utf-8");

                yield return loUnityWebRequest.SendWebRequest();

                if (loUnityWebRequest.result == UnityWebRequest.Result.Success) loOnResult?.Invoke(true, loUnityWebRequest.downloadHandler.text);
                else loOnResult?.Invoke(false, loUnityWebRequest.error);
            }
        }
    }

    /// <summary>
    /// Se usa para enviar cosas al servidor.
    /// </summary>
    /// <param name="lcUrl"></param>
    /// <param name="lcText"></param>
    /// <param name="loOnresult"></param>
    /// <returns></returns>
    public static IEnumerator Post(string lcUrl, string lcText, UnityAction<bool, string> loOnResult, CoroutineResult<string> loPostResult = null)
    {
        if (string.IsNullOrEmpty(lcText)) yield break;

        using (UnityWebRequest loUnityWebRequest = new UnityWebRequest(lcUrl, "POST"))
        {
            if (!string.IsNullOrWhiteSpace(Token)) loUnityWebRequest.SetRequestHeader("Authorization", "Bearer " + Token);

            byte[] loRawBody = Encoding.UTF8.GetBytes(lcText);
            using (loUnityWebRequest.uploadHandler = new UploadHandlerRaw(loRawBody))
            {
                using (loUnityWebRequest.downloadHandler = new DownloadHandlerBuffer())
                {
                    loUnityWebRequest.SetRequestHeader("Content-Type", "text/html; charset=utf-8");

                    yield return loUnityWebRequest.SendWebRequest();

                    if (loUnityWebRequest.result == UnityWebRequest.Result.Success)
                    {
                        if (loPostResult != null) loPostResult.OnCompleted(loUnityWebRequest.downloadHandler.text);
                        loOnResult?.Invoke(true, loUnityWebRequest.downloadHandler.text);
                    }
                    else
                    {
                        if (loPostResult != null) loPostResult.OnFailed();
                        loOnResult?.Invoke(false, loUnityWebRequest.error);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Se llama para obtener el token, ya sea creando una cuenta o como login.
    /// </summary>
    public static IEnumerator Post(string lcURL, UnityAction<bool, string> loOnResult, CoroutineResult<string> loPostResult, params DTO2<string, string>[] loParams)
    {
        //<< Si no tenemos token local sabemos que debemos crear cuenta.
        WaitForSeconds loWaitForSeconds = new WaitForSeconds(1);

        //<< Mandamos los datos de guardado a servidor.
        WWWForm loForm = new WWWForm();

        //<< Agregamos los elementos del from.
        foreach (DTO2<string, string> loItem in loParams)
        {
            if (loItem == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Hay un null en el Form del post, uno de los items!");
#endif
                continue;
            }

            if (string.IsNullOrWhiteSpace(loItem.Item1) || loItem.Item2 == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Hay un null en el Form del post, Url:{lcURL}, I1:{loItem.Item1}, I2:{loItem.Item2}");
#endif
                continue;
            }

            loForm.AddField(loItem.Item1, loItem.Item2);
        }

        using (UnityWebRequest loUnityWebRequest = UnityWebRequest.Post(lcURL, loForm))
        {
            if (!string.IsNullOrWhiteSpace(Token)) loUnityWebRequest.SetRequestHeader("Authorization", "Bearer " + Token);

            using (loUnityWebRequest.downloadHandler = new DownloadHandlerBuffer())
            {
                using (loUnityWebRequest.certificateHandler = new PrivateCertificateHandler())
                {
                    yield return loUnityWebRequest.SendWebRequest();

                    //<< Esperamos la descarga completa.
                    while (!loUnityWebRequest.isDone) yield return loWaitForSeconds;

                    if (loUnityWebRequest.result == UnityWebRequest.Result.Success)
                    {
                        if (loPostResult != null) loPostResult.OnCompleted(loUnityWebRequest.downloadHandler.text);
                        loOnResult?.Invoke(true, loUnityWebRequest.downloadHandler.text);
                    }
                    else
                    {
                        if (loPostResult != null) loPostResult.OnFailed(loUnityWebRequest.error);
                        loOnResult?.Invoke(false, loUnityWebRequest.error);
                    }
                }
            }
        }

        yield break;
    }

    /// <summary>
    /// Se llama para cargar una textura 2D de internet o del cache de existir.
    /// </summary>
    /// <param name="lcURL"></param>
    /// <param name="loOnResult"></param>
    /// <param name="loPostResult"></param>
    /// <param name="loParams"></param>
    /// <returns></returns>
    public static IEnumerator LoadTexture2DAsync(string lcURL, UnityAction<Texture2D> loOnResult, CoroutineResult<Texture2D> loPostResult, params DTO2<string, string>[] loParams)
    {
        //<< Revisamos si tenemos la textura ya cargada.
        if (goTextures2D.TryGetValue(lcURL, out Texture2D loTexture2D))
        {
            if (loPostResult != null) loPostResult.OnCompleted(loTexture2D);
            loOnResult?.Invoke(loTexture2D);
            yield break;
        }

        string lcCachePath = GetCachePath(lcURL);
        if (File.Exists(lcCachePath))
        {
            byte[] loImageData = File.ReadAllBytes(lcCachePath);
            loTexture2D = new Texture2D(2, 2);
            loTexture2D.LoadImage(loImageData);
            goTextures2D.Add(lcURL, loTexture2D);
            if (loPostResult != null) loPostResult.OnCompleted(loTexture2D);
            loOnResult?.Invoke(loTexture2D);
            yield break;
        }

        using (UnityWebRequest loUnityWebRequest = UnityWebRequestTexture.GetTexture(lcURL))
        {
            yield return loUnityWebRequest.SendWebRequest();

            if (loUnityWebRequest.result == UnityWebRequest.Result.Success)
            {
                loTexture2D = DownloadHandlerTexture.GetContent(loUnityWebRequest);
                byte[] loPNGData = loTexture2D.EncodeToPNG();
                File.WriteAllBytes(lcCachePath, loPNGData);
                goTextures2D.Add(lcURL, loTexture2D);
                if (loPostResult != null) loPostResult.OnCompleted(loTexture2D);
                loOnResult?.Invoke(loTexture2D);
            }
            else
            {
                if (loPostResult != null) loPostResult.OnFailed();
                loOnResult?.Invoke(null);
            }
        }

        yield break;
    }

    /// <summary>
    /// Se llama para liberar todas las texturas 2D.
    /// </summary>
    public static void DestroyTextures2D()
    {
        foreach (var loItem in goTextures2D)
        {
            if (loItem.Value == null) continue;
            Destroy(loItem.Value);
        }
        goTextures2D.Clear();
    }

    /// <summary>
    /// Nos da un path en cache donde podemos guardar las cosas usando la url.
    /// </summary>
    /// <param name="lcUrl"></param>
    /// <returns></returns>
    private static string GetCachePath(string lcUrl)
    {
        using (MD5 loMD5 = MD5.Create())
        {
            byte[] loHashBytes = loMD5.ComputeHash(Encoding.UTF8.GetBytes(lcUrl));
            string lcHash = BitConverter.ToString(loHashBytes).Replace("-", "").ToLowerInvariant();
            return Path.Combine(Application.persistentDataPath, lcHash + ".png");
        }
    }
}