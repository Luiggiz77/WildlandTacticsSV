public class UnitHealthbar : UIBlockbar
{
    /// <summary>
    /// OnEnable.
    /// </summary>
    private void OnEnable()
    {
        GameManager.AddListener(OnGameCommand);
    }

    /// <summary>
    /// OnDisable.
    /// </summary>
    private void OnDisable()
    {
        goUnitProperties = null;
        gameObject.SetActive(false);
        GameManager.RemoveListener(OnGameCommand);
    }

    /// <summary>
    /// Datos de la unidad.
    /// </summary>
    private UnitProperties goUnitProperties = null;

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ApplyGameplayEffectOnUnit:
                {
                    if (goUnitProperties == null) return;

                    int lnUnitStoredPropertiesId = (int)loParams[0];

                    //<< Si no somos la unidad selecionada regresamos.
                    if (goUnitProperties.Id != lnUnitStoredPropertiesId) return;

                    ////<< Obtenemos el tipo de efecto.
                    //GameplayEffectType loGameplayEffectType = (GameplayEffectType)loParams[1];

                    //switch (loGameplayEffectType)
                    //{
                    //    case GameplayEffectType.UnitHealth:
                    //    case GameplayEffectType.UnitHealhtUsingCallerPhysicalPower:
                    //    case GameplayEffectType.UnitHealthUsingCallerMagicalPower:
                    //    case GameplayEffectType.UnitHealhtUsingFixedPhysicalPower:
                    //    case GameplayEffectType.UnitHealthUsingFixedMagicalPower:
                    //    case GameplayEffectType.UnitHealthUsingAreaAndCapUsingAux:
                    //    case GameplayEffectType.UnitHealthNoAnimation:
                    //    case GameplayEffectType.UnitHealthUsingEnergy:
                    //        SetCount(goUnitProperties.Health);
                    //        break;

                    //    case GameplayEffectType.UnitMaxHealth:
                    //        SetMax(goUnitProperties.GetMaxHealth());
                    //        SetCount(goUnitProperties.Health);
                    //        break;

                    //    default: break;
                    //}
                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// Establecemos los datos de la unidad.
    /// </summary>
    /// <param name="loUnitProperties"></param>
    public void SetUnitProperties(UnitProperties loUnitProperties)
    {
        goUnitProperties = loUnitProperties;
    }
}