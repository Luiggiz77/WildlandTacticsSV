public class GameplayItemStored
{
    /// <summary>
    /// Es el Id del item.
    /// </summary>
    public int GameplayItemId;

    /// <summary>
    /// Cantidad de items disponibles.
    /// </summary>
    public int Count = 0;

    /// <summary>
    /// Nos indica cuantas veces se ha usado el objeto.
    /// </summary>
    public int Uses = 0;

    /// <summary>
    /// Nos indica si el item está seleccionado para ser usado.
    /// </summary>
    private bool gbSelected = false;

    /// <summary>
    /// Establecemos si está seleccionado o no el item.
    /// </summary>
    /// <param name="lbSelected"></param>
    public void SetSelected(bool lbSelected)
    {
        gbSelected = lbSelected;
    }

    /// <summary>
    /// Nos indica si está seleccionado el objeto. Nota: Solo aplica para items que se usan antes del combate.
    /// </summary>
    /// <returns></returns>
    public bool IsSelected()
    {
        return gbSelected;
    }
}