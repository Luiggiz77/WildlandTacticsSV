using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIGameplayItem : MonoBehaviour
{
    [Tooltip("Icono del item.")]
    public RawImage itemIcon;

    [Tooltip("Nombre del item.")]
    public TMP_Text itemName;

    /// <summary>
    /// Id del item.
    /// </summary>
    private int gnGameplayItemId = 0;

    /// <summary>
    /// Nos da el id del item.
    /// </summary>
    /// <returns></returns>
    public int GetGameplayItemId()
    {
        return gnGameplayItemId;
    }

    /// <summary>
    /// Se llama para establecer el id del objeto.
    /// </summary>
    /// <param name="lnGameplayItemId"></param>
    public virtual void SetGameplayItemId(int lnGameplayItemId)
    {
        gnGameplayItemId = lnGameplayItemId;
        itemIcon.gameObject.SetActive(false);
        itemName?.gameObject.SetActive(false);

        if (gnGameplayItemId <= 0) return;

        //<< Pedimos el icono del item.
        GameManager.Send(GameCommand.GetTexture2D, new UnityAction<Texture2D>(OnGetGameplayItemIcon), GameTexture2DUsage.GameplayItemIcon, lnGameplayItemId);

        //<< Asignamos el nombre del item.
        if(itemName != null)
        {
            itemName.gameObject.SetActive(true);
            itemName.text = GameManager.GetText(GameTextUsage.GameplayItemName, gnGameplayItemId);
        }
    }

    /// <summary>
    /// Se llama para obtener el icono de nuestra unidad.
    /// </summary>
    /// <param name="loTexture2D"></param>
    private void OnGetGameplayItemIcon(Texture2D loTexture2D)
    {
        itemIcon.gameObject.SetActive(true);
        itemIcon.texture = loTexture2D;
    }
}
