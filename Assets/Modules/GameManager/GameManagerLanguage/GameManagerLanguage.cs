using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("Language")]

    [Tooltip("Lenguaje por defecto.")]
    public LanguageWrapper language;

    /// <summary>
    /// Es una clase con los tags que se pueden usar para reemplazar dichos tags por otro texto o dato.
    /// </summary>
    internal class TextTags
    {
        internal static string Name = "{purchasable}";
        internal static string Cost = "{cost}";
        internal static string Currency = "{currencyIcon}";
    }

    /// <summary>
    /// Es el objeto que contiene los textos del lenguaje.
    /// </summary>
    private static Language goLanguage = null;

    /// <summary>
    /// Diccionario de textos. Nota: El primer valor del tuple contiene "Enum.GameTextUsage" y el segundo es el entero del enum, por ejemplo del "Enum.GameText.StartGame".
    /// </summary>
    private static Dictionary<int, Dictionary<int, string>> goTexts = new Dictionary<int, Dictionary<int, string>>();

    public static Language GetLanguage()
    {
        return goLanguage;
    }

    /// <summary>
    /// Nos da la llave del lenguaje en playerprefs.
    /// </summary>
    /// <returns></returns>
    private static string GetLanguageKey(int lnSystemLanguage)
    {
        return $"{nameof(GetLanguageKey)}{lnSystemLanguage}";
    }

    /// <summary>
    /// Se llama en el awake.
    /// </summary>
    private void AwakeLanguage()
    {
        //<< Revisamos si tenemos ya descargados los textos del idioma.
        string lcLanguageTextsKey = GetLanguageKey((int)Application.systemLanguage);

        //<< Convertimos a objeto de tener ya un json.
        if (PlayerPrefs.HasKey(lcLanguageTextsKey)) goLanguage = JsonUtility.FromJson<Language>(PlayerPrefs.GetString(lcLanguageTextsKey));

        //<< Si no hubo lenguaje solo cargamos el default.
        if (goLanguage == null) goLanguage = language.data;

        //<< Rellenamos diccionario de textos.
        FillDictionaryTexts();
    }

    /// <summary>
    /// Rellenamos los textos en el diccionario.
    /// </summary>
    private static void FillDictionaryTexts()
    {
        if (goLanguage == null) return;
        goTexts.Clear();

        GameTextUsage[] loGameTextUsages = (GameTextUsage[])System.Enum.GetValues(typeof(GameTextUsage));
        foreach (GameTextUsage loGameTextUsage in loGameTextUsages) goTexts.Add((int)loGameTextUsage, new Dictionary<int, string>());

        foreach (LanguageText loItem in goLanguage.LT) goTexts[loItem.U].Add(loItem.K, loItem.T);
    }

    /// <summary>
    /// Nos da el texto requerido.
    /// </summary>
    /// <param name="loGameText"></param>
    /// <returns></returns>
    public static string GetText(GameTextUsage loUsage, int lnText)
    {
        int lnUsage = (int)loUsage;
        if (!goTexts.ContainsKey(lnUsage) || !goTexts[lnUsage].ContainsKey(lnText))
        {
            switch (loUsage)
            {
                case GameTextUsage.Word: return ((GameTextWord)lnText).ToString();
                case GameTextUsage.Sentence: return ((GameTextSentence)lnText).ToString();
                case GameTextUsage.UnitRole: return ((UnitRole)lnText).ToString();
                default: break;
            }
            return string.Empty;
        }
        return goTexts[lnUsage][lnText];
    }

    /// <summary>
    /// Nos da el texto requerido.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="loUsage"></param>
    /// <param name="loEnum"></param>
    /// <returns></returns>
    public static string GetText<TEnum>(GameTextUsage loUsage, TEnum loEnum) where TEnum : struct, System.Enum
    {
        return GetText(loUsage, System.Convert.ToInt32(loEnum));
    }

    /// <summary>
    /// Nos indica si tenemos lenguaje.
    /// </summary>
    /// <returns></returns>
    private static bool HasLanguage()
    {
        return goLanguage != null && goLanguage.LT != null && goLanguage.LT.Count > 0;
    }
}