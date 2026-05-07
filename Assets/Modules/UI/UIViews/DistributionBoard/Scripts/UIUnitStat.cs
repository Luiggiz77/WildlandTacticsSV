using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitStat : MonoBehaviour
{
    [Tooltip("Ícono de la estadística")]
    public Image icon;

    [Tooltip("Valor de la estadística")]
    public TMP_Text value;


    public void SetStat(Sprite loIcon, string loValue)
    {
        icon.sprite = loIcon;
        value.text = loValue;
    }
}
