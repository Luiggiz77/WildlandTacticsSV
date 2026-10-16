using TMPro;
using UnityEngine;

public class UICombatUnitStats : UIControl
{
    [Tooltip("Nos indica si el control es del usuario o del oponente.")]
    public bool isOpponent = false;

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
    /// ONGameCommand
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.SetSelectedUnit:
                {
                    int lnUserId = (int)loParams[0];
                    BoardUnit loBoardUnit = (BoardUnit)loParams[1];

                    //<< Si no somos de la misma clase de unidad regresamos.
                    if (isOpponent && loBoardUnit.UserId == lnUserId) return;

                    //<< Actualizamos los stats de la unidad.
                    UpdateUnitStats(loBoardUnit);
                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// Se llama para actualizar los stats de la unidad.
    /// </summary>
    /// <param name="loBoardUnit"></param>
    private void UpdateUnitStats(BoardUnit loBoardUnit)
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
