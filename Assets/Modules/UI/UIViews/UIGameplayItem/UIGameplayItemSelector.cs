using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGameplayItemSelector : UIGameplayItem, IPointerClickHandler
{
    [Tooltip("Imagen que nos indica que el item está seleccionado.")]
    public Image selected;

    [Tooltip("LayoutElement para poner el ancho.")]
    public LayoutElement layoutElement;

    /// <summary>
    /// Se llama cuando le dan click al item y debemos mostrar la descripción del mismo.
    /// </summary>
    [HideInInspector]
    public UnityAction<int> OnPointerClickGameplayItem = null;

    /// <summary>
    /// Se llama para establecer el id del objeto.
    /// </summary>
    /// <param name="lnGameplayItemId"></param>
    public override void SetGameplayItemId(int lnGameplayItemId)
    {
        base.SetGameplayItemId(lnGameplayItemId);

        //<< Preguntamos si el item está seleccionado.
        GameManager.Send(GameCommand.GetGameplayItemSelection, lnGameplayItemId, false, new UnityAction<bool>(OnGetGameplayItemSelection));
    }

    /// <summary>
    /// Se llama cuando le hacen click al elemento.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //<< Avisamos que deseamos usar el item.
        GameManager.Send(GameCommand.SetGameplayItemSelection, GetGameplayItemId(), false, new UnityAction<bool>(OnGetGameplayItemSelection));
    }

    /// <summary>
    /// Se llama para indicarnos si está seleccionado o no.
    /// </summary>
    private void OnGetGameplayItemSelection(bool lbUsed)
    {
        selected.gameObject.SetActive(lbUsed);

        //<< Avisamos que han dado click a este item.
        OnPointerClickGameplayItem.Invoke(GetGameplayItemId());
    }
}
