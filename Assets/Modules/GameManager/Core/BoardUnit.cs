using System.Collections.Generic;

public class BoardUnit
{
    public int UnitBattlePropertiesId;

    public int UnitPropertiesId;

    public List<UnitGameplayEffect> UnitGameplayEffects = new List<UnitGameplayEffect>();
}