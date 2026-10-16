using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIGameplayItemBattle : UIGameplayItem
{
    [Tooltip("Botón del item")]
    public Button button;

    [Tooltip("Cantidad de items")]
    public TMP_Text itemCount;

    [Tooltip("Es layoutelement para asignar la altura preferida.")]
    public LayoutElement layoutElement;

    [HideInInspector]
    public UnityAction<UIGameplayItemBattle> OnClicked;

    /// <summary>
    /// Cantidad de items.
    /// </summary>
    private int gnCount;

    /// <summary>
    /// Se llama cuando se le da click al item.
    /// </summary>
    public void OnClick()
    {
        OnClicked.Invoke(this);
    }

    /// <summary>
    /// Nos da la cuenta de items.
    /// </summary>
    public int GetGameplayItemCount()
    {
        return gnCount;
    }

    /// <summary>
    /// Establecemos la cantidad de items a usar.
    /// </summary>
    /// <param name="lnCount"></param>
    public void SetGameplayItemCount(int lnCount)
    {
        gnCount = lnCount;
        itemCount.text = $"x{gnCount}";
        button.interactable = gnCount > 0;
    }
}