using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlockbar : MonoBehaviour
{
    [Tooltip("Es el prefab separador del bloques de vida.")]
    public Image prefabChild;

    /// <summary>
    /// Nos indica la cantidad de bloques maxima.
    /// </summary>
    private int gnMax = 0;

    /// <summary>
    /// Nos indican los bloques activos.
    /// </summary>
    private int gnCount = 0;

    List<Image> goChilds = new List<Image>();

    /// <summary>
    /// Establece los bloques de vida maxima. Nota: Esta funcion debe llamarse antes que <see cref="SetCount(int)"/>
    /// </summary>
    public void SetMax(int lnMax)
    {
        gnMax = lnMax;

        int lnHashCodeChild = prefabChild.GetHashCode();

        //<< Desactivamos los prefabs de los bloques de vida instanciados previamente por nosotros.
        foreach (Image loItem in goChilds) loItem.gameObject.SetActive(false);

        //<< Creamos tantos bloques como nos pidan.
        Image loChild;
        for (int i = 0; i < lnMax; i++)
        {
            loChild = GameManager.Spawn(lnHashCodeChild, prefabChild);
            loChild.enabled = true;
            loChild.transform.SetParent(transform, false);
            goChilds.Add(loChild);
        }
    }

    /// <summary>
    /// Establece la cantidad de bloques.
    /// </summary>
    public virtual void SetCount(int lnBlocks)
    {
        gnCount = lnBlocks;
        for (int i = gnMax - 1; i >= gnMax - lnBlocks; i--) goChilds[i].enabled = false;
    }
}