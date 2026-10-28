using TMPro;
using UnityEngine;

public class UICombatUnitPortrait : MonoBehaviour
{
    [Header("INFORMATION")]
    [Tooltip("Name.")]
    public TMP_Text textUnitName;

    [Tooltip("Level.")]
    public TMP_Text textUnitLevel;

    /// <summary>
    /// Se llama para actualizar la información de la unidad.
    /// </summary>
    /// <param name="loBoardUnit"></param>
    public void UpdateUnitInformation(BoardUnit loBoardUnit)
    {
        textUnitName.text = loBoardUnit.UnitProperties.Name;
        textUnitLevel.text = loBoardUnit.UnitProperties.Id.ToString();
    }
}
