using UnityEngine;

public class UIDistributionBoardCell : UIUnit
{
    [Tooltip("Es el collider del fondo de nuestro espacio el cual recibe otros coliders para identificar cualquier movimiento de unidades.")]
    public BoxCollider2D boxCollider2DBackground;

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
                {
                    int lnUnitPropertiesId = (int)loParams[0];
                    if (lnUnitPropertiesId != GetUnitId()) return;
                    int lnDistributionBoardId = (int)loParams[1];
                    if (lnDistributionBoardId != GetDistributionBoardId()) return;
                    SetUnitPropertiesId(0);
                }
                break;

            case GameCommand.AddedUnitToDistributionBoard:
                {
                    int lnDistributionBoardId = (int)loParams[1];
                    if (lnDistributionBoardId != GetDistributionBoardId()) return;
                    int lnX = (int)loParams[2];
                    if (lnX != GetX()) return;
                    int lnZ = (int)loParams[3];
                    if (lnZ != GetZ()) return;
                    SetUnitPropertiesId((int)loParams[0]);
                }
                break;

            default: break;
        }
    }
}