using TMPro;
using UnityEngine;

public class UICombatUnitStats : MonoBehaviour
{
    [Header("STATS")]
    [Tooltip("Ataque magico.")]
    public TMP_Text magicalDamage;

    [Tooltip("Ataque fisico.")]
    public TMP_Text physicalDamage;

    [Tooltip("Defensa magica.")]
    public TMP_Text magicalResistance;

    [Tooltip("Defensa fisica.")]
    public TMP_Text physicalResistance;

    [Tooltip("Control de la barra de energia.")]
    public UIBlockbar energybar;

    /// <summary>
    /// Se llama para actualizar los stats de la unidad.
    /// </summary>
    /// <param name="loBoardUnit"></param>
    public void UpdateUnitStats(BoardUnit loBoardUnit)
    {
        magicalDamage.text = loBoardUnit.GetMagicalPower().ToString();
        physicalDamage.text = loBoardUnit.GetPhysicalPower().ToString();
        magicalResistance.text = loBoardUnit.GetMagicalResistance().ToString();
        physicalResistance.text = loBoardUnit.GetPhysicalResistance().ToString();

        energybar.SetMax(loBoardUnit.GetMaxEnergy());
        energybar.SetCount(loBoardUnit.Energy);

        int lnMagicalPowerAdd = loBoardUnit.GetMagicalPowerAdd();
        int lnPhysicalPowerAdd = loBoardUnit.GetPhysicalPowerAdd();
        int lnMagicalResistanceAdd = loBoardUnit.GetMagicalResistanceAdd();
        int lnPhysicalResistanceAdd = loBoardUnit.GetPhysicalResistanceAdd();
        int lnMovementAdd = loBoardUnit.GetMovementAdd();
        int lnAttackRangeAdd = loBoardUnit.GetAttackRangeAdd();
    }
}
