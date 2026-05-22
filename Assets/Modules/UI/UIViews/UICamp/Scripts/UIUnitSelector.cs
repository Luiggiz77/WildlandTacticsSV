using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUnitSelector : UIUnit
{
    [Tooltip("Imagen sobre el botón que indica que la unidad esta en el mapa.")]
    public Image selected;

    /// <summary>
    /// OnEnable
    /// </summary>
    private void OnEnable()
    {
        //<< Nos registramos al evento de comandos para saber cuando quitan una unidad de un tablero.
        GameManager.AddListener(OnGameCommand);
    }

    /// <summary>
    /// OnDisable.
    /// </summary>
    public override void OnDisable()
    {
        base.OnDisable();

        //<< Nos desregistramos al evento de comandos para saber cuando quitan una unidad de un tablero.
        GameManager.RemoveListener(OnGameCommand);
    }

    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.RemovedUnitFromDistributionBoard:
            case GameCommand.AddedUnitToDistributionBoard:
                int lnUnitPropertiesId = (int)loParams[0];
                if (lnUnitPropertiesId == GetUnitId()) GameManager.Send(GameCommand.UnitExistsOnDistributionBoard, lnUnitPropertiesId, new UnityAction<bool>(UnitExistsOnDistributionBoard));
                break;
            default: break;
        }
    }

    /// <summary>
    /// Se llama para establecer el id de la unidad.
    /// </summary>
    /// <param name="lnUnitPropertiesId"></param>
    public override void SetUnitPropertiesId(int lnUnitPropertiesId)
    {
        base.SetUnitPropertiesId(lnUnitPropertiesId);

        //<< Preguntamos si la unidad se encuentra en algun tablero de distribución.
        GameManager.Send(GameCommand.UnitExistsOnDistributionBoard, lnUnitPropertiesId, new UnityAction<bool>(UnitExistsOnDistributionBoard));
    }

    /// <summary>
    /// Action que se llama cuando se agrega o se quita del mapa.
    /// </summary>
    /// <param name="lbResult"></param>
    private void UnitExistsOnDistributionBoard(bool lbActive)
    {
        selected.gameObject.SetActive(lbActive);
    }
}