public class UICombat : UIView
{
    /// <summary>
    /// Para recibir comandos.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            default: break;
        }
    }
}