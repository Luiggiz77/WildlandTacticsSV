using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UIDistributionBoard : MonoBehaviour
{
    [Tooltip("Para controlar la altura de nuestro elemento.")]
    public LayoutElement layoutElement;

    [Tooltip("Para controlar el tamaño de las celdas.")]
    public GridLayoutGroup gridLayoutGroup;

    [Tooltip("Para el nombre del tablero de distribución.")]
    public TMP_Text distributionBoardName;

    [Tooltip("Bordes horizontales.")]
    public LayoutElement[] horizontalBorders;

    [Tooltip("Bordes verticales.")]
    public LayoutElement[] verticalBorders;

    /// <summary>
    /// Id del tablero de distribución.
    /// </summary>
    [HideInInspector]
    public int gnDistributionBoardId = 0;

    /// <summary>
    /// Se llama cuando le dan click al nombre para editarlo, aqui mostramos el globo de edición.
    /// </summary>
    public void OnClickName()
    {
        Debug.Log("gnDistributionBoardId: " + gnDistributionBoardId);
        GameManager.Send(GameCommand.ShowUIEditStringBalloon, distributionBoardName.text, new UnityAction<string>(OnEditedString), nameof(DistributionBoard), nameof(DistributionBoard.Name), gnDistributionBoardId, "False");
    }

    /// <summary>
    /// Se llama cuando se terminó de editar el nombre.
    /// </summary>
    private void OnEditedString(string lcString)
    {
        distributionBoardName.text = lcString;
    }
}