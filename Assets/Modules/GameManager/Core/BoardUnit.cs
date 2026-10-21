using System;
using System.Collections.Generic;

[Serializable]
public class BoardUnit
{
    /// <summary>
    /// Id del jugador a quien pertenece.
    /// </summary>
    public int UserId = 0;

    /// <summary>
    /// Indica la vida maxima actual de la unidad.
    /// </summary>
    public int HealthMax = 0;

    /// <summary>
    /// Indica la vida actual de la unidad.
    /// </summary>
    public int Health = 0;

    /// <summary>
    /// Indica el nivel de energia de la unidad.
    /// </summary>
    public int Energy = 0;

    /// <summary>
    /// Indica la cantidad de ataques que ha realizado la unidad.
    /// </summary>
    public int Attacks = 0;

    /// <summary>
    /// Indica la cantidad de movimientos que ha realizado la unidad.
    /// </summary>
    public int Moves = 0;

    /// <summary>
    /// Direccion en la que ve la unidad.
    /// </summary>
    public Facing Facing;

    public UnitProperties UnitProperties;

    /// <summary>
    /// Lista de efectos usados para el ataque de la unidad.
    /// </summary>
    public List<GameplayEffect> GameplayEffectsAttack = new List<GameplayEffect>();

    /// <summary>
    /// Lista de efectos agregados a la unidad.
    /// </summary>
    public List<GameplayEffect> GameplayEffects = new List<GameplayEffect>();

    /// <summary>
    /// Nos da el daño físico actual de la unidad actual contemplando efectos sobre ésta unidad.
    /// </summary>
    /// <returns></returns>
    public int GetPhysicalPower()
    {
        int lnPhysicalPower = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.PhysicalPower;
        return lnPhysicalPower + GetPhysicalPowerAdd();
    }

    /// <summary>
    /// Nos da el daño mágico actual de la unidad actual contemplando efectos sobre ésta unidad.
    /// </summary>
    /// <returns></returns>
    public int GetMagicalPower()
    {
        int lnMagicalPower = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.MagicalPower;
        return lnMagicalPower + GetMagicalPowerAdd();
    }

    /// <summary>
    /// Nos da el daño físico actual de la unidad actual contemplando efectos sobre ésta unidad.
    /// </summary>
    /// <returns></returns>
    public int GetPhysicalResistance()
    {
        int lnPhysicalResistance = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.PhysicalResistance;
        return lnPhysicalResistance + GetPhysicalResistanceAdd();
    }

    /// <summary>
    /// Nos da el daño mágico actual de la unidad actual contemplando efectos sobre ésta unidad.
    /// </summary>
    /// <returns></returns>
    public int GetMagicalResistance()
    {
        int lnMagicalResistance = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.MagicalResistance;
        return lnMagicalResistance + GetMagicalResistanceAdd();
    }

    /// <summary>
    /// Nos da el movimiento actual de la unidad actual contemplando efectos sobre ésta unidad.
    /// </summary>
    /// <returns></returns>
    public int GetMovement()
    {
        //<< Revisamos si tenemos el efecto que reduce el movimiento a N.
        foreach (GameplayEffect loItem in GameplayEffects)
        {
            if (loItem.Type != GameplayEffectType.FixedUnitMovement) continue;
            return loItem.Value;
        }

        int lnMovement = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.Movement;
        lnMovement += GetMovementAdd();
        if (lnMovement < 0) return 0;
        return lnMovement;
    }

    /// <summary>
    /// Nos da el rango de ataque actual de la unidad actual contemplando efectos sobre ésta unidad.
    /// </summary>
    /// <returns></returns>
    public int GetAttackRange()
    {
        int lnAttackRange = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.AttackRange;
        lnAttackRange += GetAttackRangeAdd();
        if (lnAttackRange < 0) return 0;
        return lnAttackRange;
    }

    /// <summary>
    /// Nos da el hp maximo de la unidad tomando en cuenta los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMaxHealth()
    {
        int lnHealth = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.Health;
        return lnHealth + GetMaxHealthAdd();
    }

    /// <summary>
    /// Nos da el hp maximo adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMaxHealthAdd()
    {
        return GetValueOf(GameplayEffectType.UnitMaxHealth);
    }

    /// <summary>
    /// Nos da la energia maxima de la unidad tomando en cuenta los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMaxEnergy()
    {
        int lnEnergy = UnitProperties == null ? 0 : UnitProperties.UnitBattleProperties == null ? 0 : UnitProperties.UnitBattleProperties.Energy;
        return lnEnergy + GetMaxEnergyAdd();
    }

    /// <summary>
    /// Nos da la energia maxima adivita de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMaxEnergyAdd()
    {
        return GetValueOf(GameplayEffectType.UnitMaxEnergy);
    }

    /// <summary>
    /// Nos da el poder magico adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMagicalPowerAdd()
    {
        return GetValueOf(GameplayEffectType.UnitMagicalPower);
    }

    /// <summary>
    /// Nos da el poder fisico adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetPhysicalPowerAdd()
    {
        return GetValueOf(GameplayEffectType.UnitPhysicalPower);
    }

    /// <summary>
    /// Nos da el poder magico adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMagicalResistanceAdd()
    {
        return GetValueOf(GameplayEffectType.UnitMagicalResistance);
    }

    /// <summary>
    /// Nos da el poder fisico adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetPhysicalResistanceAdd()
    {
        return GetValueOf(GameplayEffectType.UnitPhysicalResistance);
    }

    /// <summary>
    /// Nos da el movimiento adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetMovementAdd()
    {
        int lnMovement = 0;

        //<< Revisamos si tenemos el efecto que reduce el movimiento a N.
        foreach (GameplayEffect loItem in GameplayEffects)
        {
            //<< Si los puntos de movimiento son fijos mandamos el valor asignado.
            if (loItem.Type == GameplayEffectType.UnitMovement) lnMovement += loItem.Value;
        }

        return lnMovement;
    }

    /// <summary>
    /// Nos da el rango de ataque adivito de la unidad de solo los efectos activos.
    /// </summary>
    /// <returns></returns>
    public int GetAttackRangeAdd()
    {
        return GetValueOf(GameplayEffectType.UnitAttackRange);
    }

    /// <summary>
    /// Nos da el valor de la sumatiria del tipo de efecto.
    /// </summary>
    /// <param name="loUnitGameplayEffects"></param>
    /// <param name="loGameplayEffectType"></param>
    /// <returns></returns>
    private int GetValueOf(GameplayEffectType loGameplayEffectType)
    {
        int lnValue = 0;
        foreach (GameplayEffect loItem in GameplayEffects)
        {
            if (loItem.Type != loGameplayEffectType) continue;
            lnValue += loItem.Value;
        }
        return lnValue;
    }
}