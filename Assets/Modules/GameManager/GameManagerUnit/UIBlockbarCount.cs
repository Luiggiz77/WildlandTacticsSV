using TMPro;
using UnityEngine;

public class UIBlockbarCount : UIBlockbar
{
    [Tooltip("Conteo de bloques.")]
    public TMP_Text count;

    /// <summary>
    /// Establecemos la cantidad de bloques.
    /// </summary>
    /// <param name="lnCount"></param>
    public override void SetCount(int lnCount)
    {
        base.SetCount(lnCount);

        //<< Indicamos cuantos bloques tenemos.
        count.text = lnCount.ToString();
    }
}
