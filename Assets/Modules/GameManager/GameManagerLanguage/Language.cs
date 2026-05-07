using System;
using System.Collections.Generic;

[Serializable]
public class Language
{
    /// <summary>
    /// Es el codigo del lenguaje
    /// Application.systemLanguage
    /// </summary>
    public int L;

    /// <summary>
    /// Indica la versión.
    /// </summary>
    public int V;

    /// <summary>
    /// Lista de textos del lenguaje.
    /// </summary>
    public List<LanguageText> LT;
}