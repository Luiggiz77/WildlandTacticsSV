using System;

[Serializable]
public class LanguageText
{
    /// <summary>
    /// Es el tipo de texto del juego, UI, Items, MapItems, etc... (Enum.GameTextUsage)
    /// </summary>
    public int U;

    /// <summary>
    /// Es la llave representativa del texto, ejemplo:  (int)GameText.StartGame, (int)ItemType.Body, etc...
    /// </summary>
    public int K;

    /// <summary>
    /// es el texto que se aplicará.
    /// </summary>
    public string T;
}