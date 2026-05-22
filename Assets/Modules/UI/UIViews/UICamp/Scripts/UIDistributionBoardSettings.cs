using UnityEngine;

[CreateAssetMenu(menuName = "GamePlay/UI/UIDistributionBoardSettings")]
public class UIDistributionBoardSettings : ScriptableObject
{
    [Tooltip("Indica la altura normalizada que ocupan los nombres de los tableros de distribución en el UI."), Range(0.0f, 1.0f)]
    public float nameHeight = 0.05f;

    [Tooltip("Indica el ancho normalizado que ocupan los bordes de los tableros de distribución en el UI."), Range(0.0f, 1.0f)]
    public float borderThickness = 0.01f;
}
