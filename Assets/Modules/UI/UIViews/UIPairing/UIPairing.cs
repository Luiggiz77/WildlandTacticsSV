public class UIPairing : UIView
{
    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUIPairing: panel.SetActive(true); 
                //<< TODO: Aqui deberiamos recibir los datos del jugador inicial a mostrar y despues al conectarse otro jugador se muestran los de el en otro mensaje.
                break;
            case GameCommand.HideUIPairing: panel.SetActive(false); break;
            default: break;
        }
    }

    /// <summary>
    /// Se llama cuando le dan click a l boton de cancelar busqueda.
    /// </summary>
    public void OnClickCancel()
    {
        GameManager.Send(GameCommand.CancelPairing);
    }
}
