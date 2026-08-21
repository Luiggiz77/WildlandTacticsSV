using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlockbar : MonoBehaviour
{
    [Tooltip("Es la imagen que se usa para el fill.")]
    public Image barFill;

    [Tooltip("Es el prefab separador del bloques de vida.")]
    public LayoutElement prefabSeparator;

    [Tooltip("Es el prefab relleno del bloques de vida.")]
    public LayoutElement prefabPadding;

    [Tooltip("Es el que será el padre de los prefabs de separadores y rellenos.")]
    public Transform prefabsParent;

    /// <summary>
    /// Lista separadores instanciados.
    /// </summary>
    private List<LayoutElement> goSeparators = new List<LayoutElement>();

    /// <summary>
    /// Lista de rellenos instanciados.
    /// </summary>
    private List<LayoutElement> goPaddings = new List<LayoutElement>();

    /// <summary>
    /// Nos indica la cantidad de bloques maxima.
    /// </summary>
    private int gnMax = 0;

    /// <summary>
    /// Nos indican los bloques activos.
    /// </summary>
    private int gnCount = 0;

    /// <summary>
    /// Establece los bloques de vida maxima. Nota: Esta funcion debe llamarse antes que <see cref="SetCount(int)"/>
    /// </summary>
    public void SetMax(int lnMax)
    {
        gnMax = lnMax;

        //<< Desactivamos los prefabs de los bloques de vida instanciados previamente por nosotros.
        foreach (LayoutElement loItem in goSeparators) loItem.gameObject.SetActive(false);
        foreach (LayoutElement loItem in goPaddings) loItem.gameObject.SetActive(false);
        goSeparators.Clear();
        goPaddings.Clear();

        int lnHashCodeSeparator = prefabSeparator.GetHashCode();
        int lnHashCodePadding = prefabPadding.GetHashCode();

        //<< Creamos tantos separadores como bloques de vida menos 1.
        LayoutElement loSeparator;
        LayoutElement loPadding;
        int lnCount = lnMax - 1;
        for (int i = 0; i < lnCount; i++)
        {
            loPadding = GameManager.Spawn(lnHashCodePadding, prefabPadding);
            loPadding.transform.SetParent(prefabsParent, false);
            goPaddings.Add(loPadding);

            loSeparator = GameManager.Spawn(lnHashCodeSeparator, prefabSeparator);
            loSeparator.transform.SetParent(prefabsParent, false);
            goSeparators.Add(loSeparator);
        }

        loPadding = GameManager.Spawn(lnHashCodePadding, prefabPadding);
        loPadding.transform.SetParent(prefabsParent, false);
        goPaddings.Add(loPadding);

        UpdateCount();
    }

    /// <summary>
    /// Establece la cantidad de bloques.
    /// </summary>
    public virtual void SetCount(int lnBlocks)
    {
        gnCount = lnBlocks;
        UpdateCount();
    }

    /// <summary>
    /// Se llama para actualizar la vida.
    /// </summary>
    private void UpdateCount()
    {
        barFill.fillAmount = (float)gnCount / gnMax;
    }
}