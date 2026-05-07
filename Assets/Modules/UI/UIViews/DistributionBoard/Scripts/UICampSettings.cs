using UnityEngine;

[CreateAssetMenu(menuName = "GamePlay/UI/Screen/UICampSettings")]
public class UICampSettings : ScriptableObject
{
    [Tooltip("Indica la cantidad de columnas que se deben mostrar en el grid.")]
    public int gridColumns = 5;

    [Tooltip("Indica la el ancho normalizado que ocupa el área de selección de unidades para permitir padding."), Range(0.0f, 1.0f)]
    public float unitSelectorWidth = 0.7f;

    [Tooltip("Indica la altura normalizada que ocupan los tableros de distribución en el UI."), Range(0.0f, 1.0f)]
    public float distributionBoardHeight = 0.7f;

    [Tooltip("Indica el ancho normalizado que ocupan los tableros de distribución en el UI."), Range(0.0f, 1.0f)]
    public float distributionBoardWidth = 0.9f;
}
